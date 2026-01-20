# PHÂN TÍCH VẤN ĐỀ PERFORMANCE VÀ GIẢI PHÁP

## 🔴 VẤN ĐỀ 1: MEMORY LEAK - DbContext KHÔNG ĐƯỢC DISPOSE

### Nguyên nhân:
- Mỗi view tạo `BookstoreDbContext` và `UnitOfWork` mới trong constructor
- Không implement `IDisposable` để dispose resources
- DbContext giữ connection pool và tracking entities → Memory leak + Connection pool exhaustion

### Vị trí:
- PaymentView.xaml.cs (dòng 153-154)
- InvoiceView.xaml.cs (dòng 37-38)  
- ImportDetailView.xaml.cs (dòng 37-38)
- OrderDetailView.xaml.cs (dòng 36-37)

### Giải pháp:
Implement IDisposable và dispose DbContext/UnitOfWork khi view bị hủy.

---

## 🔴 VẤN ĐỀ 2: LOAD TẤT CẢ DỮ LIỆU CÙNG LÚC

### Nguyên nhân:
- PaymentView: Load TẤT CẢ books và customers ngay khi khởi tạo
- InvoiceView: Load TẤT CẢ orders và imports cùng lúc
- Không có pagination, lazy loading, hoặc virtual scrolling

### Vị trí:
- PaymentView.xaml.cs: `LoadProductsAsync()` - load tất cả books
- PaymentView.xaml.cs: `LoadCustomersAsync()` - load tất cả customers
- InvoiceViewModel.cs: `LoadAllDataAsync()` - load tất cả orders + imports

### Giải pháp:
- Implement pagination hoặc lazy loading
- Chỉ load dữ liệu khi cần (on-demand)
- Sử dụng virtual scrolling cho DataGrid

---

## 🔴 VẤN ĐỀ 3: SEARCH TRIGGER QUÁ NHIỀU (KHÔNG CÓ DEBOUNCE)

### Nguyên nhân:
- `SearchKeyword` setter gọi `SearchInvoiceCommand.Execute(null)` mỗi lần thay đổi
- `SearchText` setter gọi `FilterProductsAsync()` mỗi ký tự gõ
- Mỗi ký tự = 1 database query → Quá tải database

### Vị trí:
- InvoiceViewModel.cs (dòng 74-83)
- PaymentView.xaml.cs (dòng 59-67)

### Giải pháp:
- Implement debounce (chờ 300-500ms sau khi user ngừng gõ)
- Sử dụng Timer hoặc CancellationTokenSource

---

## 🔴 VẤN ĐỀ 4: TẠO VIEW MỚI MỖI LẦN NAVIGATE

### Nguyên nhân:
- Mỗi lần navigate tạo `InvoiceView` mới → Load lại TẤT CẢ data
- Không cache hoặc reuse view instance
- `BtnBack_Click` tạo `InvoiceView` mới mỗi lần

### Vị trí:
- ImportDetailView.xaml.cs: `BtnBack_Click()` (dòng 98)
- OrderDetailView.xaml.cs: `BtnBack_Click()` (dòng 124)
- InvoiceView.xaml.cs: Mỗi lần navigate tạo mới

### Giải pháp:
- Cache view instance hoặc reload data thay vì tạo mới
- Sử dụng singleton pattern cho các view chính

---

## 🔴 VẤN ĐỀ 5: FIRE-AND-FORGET ASYNC TRONG CONSTRUCTOR

### Nguyên nhân:
- Constructor gọi `_ = LoadProductsAsync()` và `_ = LoadCustomersAsync()` 
- Fire-and-forget → Không await → Có thể gây race condition
- Nếu có exception, không được catch

### Vị trí:
- PaymentView.xaml.cs (dòng 176-177)

### Giải pháp:
- Load data trong `Loaded` event thay vì constructor
- Hoặc sử dụng async initialization pattern

---

## 🔴 VẤN ĐỀ 6: LOAD DỮ LIỆU TRONG CONSTRUCTOR (BLOCKING)

### Nguyên nhân:
- Constructor tạo DbContext và services → Có thể block UI thread
- Nếu database chậm, UI sẽ freeze

### Giải pháp:
- Di chuyển tất cả data loading vào `Loaded` event
- Sử dụng async/await đúng cách

---

## 🔴 VẤN ĐỀ 7: KHÔNG CÓ ERROR HANDLING TỐT

### Nguyên nhân:
- Nhiều nơi không catch exception đúng cách
- Có thể crash app nếu database error

### Giải pháp:
- Wrap tất cả async operations trong try-catch
- Log errors và hiển thị user-friendly messages

---

## 🔴 VẤN ĐỀ 8: N+1 QUERY PROBLEM

### Nguyên nhân:
- Có thể xảy ra nếu service không load related entities đúng cách
- Cần kiểm tra Include() trong repositories

### Giải pháp:
- Đảm bảo sử dụng Include() để eager load related entities
- Sử dụng projection để chỉ load fields cần thiết
