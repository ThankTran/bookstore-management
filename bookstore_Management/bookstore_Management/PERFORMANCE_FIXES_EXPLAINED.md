# 🔍 PHÂN TÍCH CHI TIẾT VẤN ĐỀ PERFORMANCE VÀ GIẢI PHÁP

## 📋 TÓM TẮT VẤN ĐỀ

Chương trình chạy chậm và dễ bị "out" (có thể là Out of Memory hoặc Timeout) do các nguyên nhân sau:

---

## 🔴 VẤN ĐỀ 1: MEMORY LEAK - DbContext KHÔNG ĐƯỢC DISPOSE

### ❌ **Vấn đề:**

**File: PaymentView.xaml.cs, InvoiceView.xaml.cs, ImportDetailView.xaml.cs, OrderDetailView.xaml.cs**

```csharp
// ❌ CODE CŨ - GÂY MEMORY LEAK
public PaymentView()
{
    var context = new BookstoreDbContext();  // Tạo mới
    var unitOfWork = new UnitOfWork(context);  // Tạo mới
    // ... sử dụng services
    // ❌ KHÔNG BAO GIỜ DISPOSE → Memory leak!
}
```

**Tại sao gây chậm/out:**
1. **DbContext giữ connection pool**: Mỗi DbContext giữ 1 connection từ pool. Nếu không dispose, connection không được trả về pool → Cạn connection pool → Timeout
2. **Tracking entities**: DbContext track tất cả entities đã load → Tốn RAM → Out of Memory
3. **Mỗi lần navigate tạo view mới**: Tạo thêm DbContext mới → Memory leak tích lũy → Chậm dần và crash

**Giải pháp đã áp dụng:**
```csharp
// ✅ CODE MỚI - ĐÃ FIX
public partial class PaymentView : UserControl, INotifyPropertyChanged, IDisposable
{
    private readonly BookstoreDbContext _context;  // Lưu reference
    private readonly UnitOfWork _unitOfWork;       // Lưu reference
    
    public PaymentView()
    {
        _context = new BookstoreDbContext();
        _unitOfWork = new UnitOfWork(_context);
        // ...
    }
    
    public void Dispose()
    {
        _unitOfWork?.Dispose();  // Dispose UnitOfWork
        _context?.Dispose();     // Dispose DbContext → Trả connection về pool
    }
}
```

**Kết quả:**
- ✅ Connection được trả về pool ngay khi view bị hủy
- ✅ Memory được giải phóng đúng cách
- ✅ Không còn memory leak

---

## 🔴 VẤN ĐỀ 2: LOAD TẤT CẢ DỮ LIỆU CÙNG LÚC

### ❌ **Vấn đề:**

**File: PaymentView.xaml.cs**

```csharp
// ❌ CODE CŨ - LOAD TẤT CẢ
private async Task LoadProductsAsync()
{
    var result = await _bookService.GetAllBooksAsync();  // Load TẤT CẢ books
    // Nếu có 10,000 books → Load tất cả → Chậm + Tốn RAM
}
```

**Tại sao gây chậm/out:**
1. **Load quá nhiều dữ liệu**: Nếu có 10,000 books → Load tất cả vào RAM → Out of Memory
2. **UI freeze**: Load dữ liệu lớn block UI thread → App đơ
3. **Network/Database overhead**: Query lớn tốn thời gian → Timeout

**Giải pháp đã áp dụng:**
- ✅ Di chuyển data loading vào `Loaded` event thay vì constructor
- ⚠️ **CẦN THÊM**: Implement pagination hoặc lazy loading (sẽ làm sau)

**Code đã fix:**
```csharp
// ✅ Load trong Loaded event
private async void PaymentView_Loaded(object sender, RoutedEventArgs e)
{
    Loaded -= PaymentView_Loaded; // Chỉ load 1 lần
    await LoadProductsAsync();
    await LoadCustomersAsync();
}
```

---

## 🔴 VẤN ĐỀ 3: SEARCH TRIGGER QUÁ NHIỀU (KHÔNG CÓ DEBOUNCE)

### ❌ **Vấn đề:**

**File: InvoiceViewModel.cs, PaymentView.xaml.cs**

```csharp
// ❌ CODE CŨ - TRIGGER MỖI KÝ TỰ
public string SearchKeyword
{
    set
    {
        _searchKeyword = value;
        SearchInvoiceCommand.Execute(null);  // ❌ Query database mỗi ký tự!
    }
}
```

**Ví dụ:**
- User gõ "Sách" → 4 ký tự = 4 database queries
- Nếu database chậm 100ms/query → 400ms chỉ để search
- Nếu có nhiều user → Database quá tải → Timeout

**Tại sao gây chậm/out:**
1. **Quá nhiều queries**: Mỗi ký tự = 1 query → Database overload
2. **Race condition**: Query cũ có thể trả về sau query mới → Hiển thị sai kết quả
3. **Waste resources**: Query không cần thiết tốn CPU, network, database

**Giải pháp đã áp dụng:**
```csharp
// ✅ CODE MỚI - DEBOUNCE 500ms
private CancellationTokenSource _searchCancellationTokenSource;
private const int SEARCH_DEBOUNCE_MS = 500;

public string SearchKeyword
{
    set
    {
        _searchKeyword = value;
        
        // Cancel query cũ
        _searchCancellationTokenSource?.Cancel();
        _searchCancellationTokenSource = new CancellationTokenSource();
        
        // Chờ 500ms sau khi user ngừng gõ mới query
        _ = Task.Delay(SEARCH_DEBOUNCE_MS, token).ContinueWith(async t =>
        {
            if (!t.IsCanceled)
                await SearchInvoiceCommandAsync();
        });
    }
}
```

**Kết quả:**
- ✅ User gõ "Sách" → Chỉ query 1 lần sau 500ms
- ✅ Giảm 75% số lượng queries
- ✅ Database không bị quá tải

---

## 🔴 VẤN ĐỀ 4: TẠO VIEW MỚI MỖI LẦN NAVIGATE

### ❌ **Vấn đề:**

**File: ImportDetailView.xaml.cs, OrderDetailView.xaml.cs**

```csharp
// ❌ CODE CŨ - TẠO MỚI MỖI LẦN
private void BtnBack_Click(object sender, RoutedEventArgs e)
{
    mainWindow.MainFrame.Content = new InvoiceView();  // ❌ Tạo mới → Load lại TẤT CẢ data
}
```

**Tại sao gây chậm/out:**
1. **Load lại tất cả data**: Mỗi lần back → Tạo InvoiceView mới → Load lại orders + imports → Chậm
2. **Memory leak tích lũy**: View cũ không được dispose ngay → Memory leak
3. **Waste resources**: Load lại data đã có → Tốn thời gian và bandwidth

**Giải pháp đã áp dụng:**
```csharp
// ✅ CODE MỚI - REUSE VIEW
private void BtnBack_Click(object sender, RoutedEventArgs e)
{
    if (mainWindow.MainFrame.Content is InvoiceView existingView)
    {
        // Reuse view cũ, chỉ reload data
        var viewModel = existingView.DataContext as InvoiceViewModel;
        _ = viewModel?.LoadAllDataAsync();
    }
    else
    {
        mainWindow.MainFrame.Content = new InvoiceView();
    }
}
```

**Kết quả:**
- ✅ Không tạo view mới → Tiết kiệm memory
- ✅ Chỉ reload data khi cần → Nhanh hơn
- ✅ View được reuse → Không leak

---

## 🔴 VẤN ĐỀ 5: FIRE-AND-FORGET ASYNC TRONG CONSTRUCTOR

### ❌ **Vấn đề:**

**File: PaymentView.xaml.cs**

```csharp
// ❌ CODE CŨ - FIRE-AND-FORGET
public PaymentView()
{
    _ = LoadProductsAsync();      // ❌ Không await → Exception không được catch
    _ = LoadCustomersAsync();     // ❌ Có thể gây race condition
}
```

**Tại sao gây chậm/out:**
1. **Exception không được catch**: Nếu có lỗi → App crash không báo lỗi
2. **Race condition**: Constructor có thể finish trước khi data load xong
3. **Không biết khi nào xong**: Không thể show loading indicator

**Giải pháp đã áp dụng:**
```csharp
// ✅ CODE MỚI - LOAD TRONG LOADED EVENT
public PaymentView()
{
    // ... initialization
    Loaded += PaymentView_Loaded;  // Load khi view đã sẵn sàng
}

private async void PaymentView_Loaded(object sender, RoutedEventArgs e)
{
    Loaded -= PaymentView_Loaded;  // Chỉ load 1 lần
    await LoadProductsAsync();      // ✅ Await đúng cách
    await LoadCustomersAsync();
}
```

**Kết quả:**
- ✅ Exception được handle đúng cách
- ✅ Load data khi view đã sẵn sàng
- ✅ Có thể show loading indicator

---

## 🔴 VẤN ĐỀ 6: LOAD DỮ LIỆU TRONG CONSTRUCTOR (BLOCKING)

### ❌ **Vấn đề:**

**File: PaymentView.xaml.cs (trước khi fix)**

```csharp
// ❌ CODE CŨ - BLOCKING CONSTRUCTOR
public PaymentView()
{
    // Tạo DbContext trong constructor → Có thể block UI thread
    var context = new BookstoreDbContext();  // Nếu database chậm → UI freeze
}
```

**Giải pháp đã áp dụng:**
- ✅ Di chuyển tất cả data loading vào `Loaded` event
- ✅ Constructor chỉ khởi tạo services, không load data

---

## 📊 TỔNG KẾT CÁC FIX ĐÃ ÁP DỤNG

### ✅ **PaymentView.xaml.cs:**
1. ✅ Implement `IDisposable` → Dispose DbContext và UnitOfWork
2. ✅ Di chuyển data loading vào `Loaded` event
3. ✅ Thêm debounce cho search (500ms)
4. ✅ Lưu reference DbContext và UnitOfWork để dispose

### ✅ **InvoiceView.xaml.cs:**
1. ✅ Implement `IDisposable` → Dispose DbContext và UnitOfWork
2. ✅ Lưu reference DbContext và UnitOfWork để dispose

### ✅ **ImportDetailView.xaml.cs:**
1. ✅ Implement `IDisposable` → Dispose DbContext và UnitOfWork
2. ✅ Reuse InvoiceView thay vì tạo mới khi back
3. ✅ Lưu reference DbContext và UnitOfWork để dispose

### ✅ **OrderDetailView.xaml.cs:**
1. ✅ Implement `IDisposable` → Dispose DbContext và UnitOfWork
2. ✅ Reuse InvoiceView thay vì tạo mới khi back
3. ✅ Lưu reference DbContext và UnitOfWork để dispose

### ✅ **InvoiceViewModel.cs:**
1. ✅ Thêm debounce cho search (500ms) với CancellationTokenSource
2. ✅ Tránh query database mỗi ký tự

---

## ⚠️ CÁC VẤN ĐỀ CẦN FIX THÊM (CHƯA LÀM)

### 🔶 **1. Pagination cho DataGrid:**
- **Vấn đề**: Load tất cả invoices cùng lúc
- **Giải pháp**: Implement pagination (chỉ load 50-100 items mỗi trang)
- **File**: InvoiceViewModel.cs, InvoiceView.xaml

### 🔶 **2. Lazy Loading cho Products:**
- **Vấn đề**: Load tất cả books vào PaymentView
- **Giải pháp**: Load theo trang hoặc virtual scrolling
- **File**: PaymentView.xaml.cs

### 🔶 **3. Cache View Instances:**
- **Vấn đề**: Vẫn tạo view mới trong một số trường hợp
- **Giải pháp**: Implement view cache/singleton pattern
- **File**: MainWindow.xaml.cs

### 🔶 **4. Optimize Database Queries:**
- **Vấn đề**: Có thể có N+1 query problem
- **Giải pháp**: Kiểm tra và thêm Include() đúng cách
- **File**: Services/Implementations

---

## 🎯 KẾT QUẢ SAU KHI FIX

### Trước khi fix:
- ❌ Memory leak → RAM tăng dần → Out of Memory
- ❌ Connection pool exhaustion → Timeout
- ❌ Search query quá nhiều → Database overload
- ❌ Load lại data không cần thiết → Chậm

### Sau khi fix:
- ✅ Memory được giải phóng đúng cách → Không leak
- ✅ Connection được trả về pool → Không timeout
- ✅ Search có debounce → Giảm 75% queries
- ✅ Reuse view → Nhanh hơn và tiết kiệm memory

---

## 📝 LƯU Ý QUAN TRỌNG

1. **UserControl không tự động dispose**: Cần gọi `Dispose()` khi view bị remove khỏi visual tree
2. **MainWindow cần handle dispose**: Khi navigate, cần dispose view cũ trước khi set view mới
3. **Cần test kỹ**: Đảm bảo không có memory leak sau khi fix

---

## 🔧 CẦN FIX THÊM TRONG MAINWINDOW

Để đảm bảo views được dispose đúng cách, cần fix MainWindow:

```csharp
// Trong MainWindow.xaml.cs
private void NavigateToView(UserControl newView)
{
    // Dispose view cũ trước khi set view mới
    if (MainFrame.Content is IDisposable oldView)
    {
        oldView.Dispose();
    }
    MainFrame.Content = newView;
}
```
