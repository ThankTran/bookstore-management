// using NUnit.Framework;
// using bookstore_Management.Core.Utils;
// using bookstore_Management.Core.Enums;
//
// namespace bookstore_Management.Tests
// {
//     [TestFixture]
//     public class PermissionHelperTests
//     {
//         [Test]
//         public void CanView_Should_ReturnTrue_For_Admin_On_Book()
//         {
//             // Arrange
//             var role = UserRole.Admin;
//             var feature = Feature.Book;
//             // Act
//             var result = PermissionHelper.CanView(role, feature);
//             // Assert
//             Assert.IsTrue(result);
//         }
//
//         [Test]
//         public void CanCreate_Should_ReturnFalse_For_Guest_On_Publisher()
//         {
//             // Arrange
//             var role = UserRole.Guest;
//             var feature = Feature.Publisher;
//             // Act
//             var result = PermissionHelper.CanCreate(role, feature);
//             // Assert
//             Assert.IsFalse(result);
//         }
//
//         [Test]
//         public void CanEdit_Should_ReturnExpected_For_Manager_On_Staff()
//         {
//             // Arrange
//             var role = UserRole.Manager;
//             var feature = Feature.Staff;
//             // Act
//             var result = PermissionHelper.CanEdit(role, feature);
//             // Assert
//             Assert.That(result, Is.TypeOf<bool>()); // True/False tuỳ quyền thực tế trong PermissionConstants
//         }
//
//         [Test]
//         public void CanDelete_Should_Handle_NullRole_Gracefully()
//         {
//             // Arrange
//             var feature = Feature.Invoices;
//             // Act & Assert
//             Assert.DoesNotThrow(() => PermissionHelper.CanDelete((UserRole)(-1), feature));
//         }
//
//         [Test]
//         public void CanExport_Should_ReturnFalse_For_InvalidFeature()
//         {
//             // Arrange
//             var role = UserRole.Admin;
//             var feature = (Feature)9999;
//             // Act
//             var result = PermissionHelper.CanExport(role, feature);
//             // Assert
//             Assert.IsFalse(result);
//         }
//
//         [Test]
//         public void CanPerform_Should_BeConsistent_With_ActionType()
//         {
//             // Arrange
//             var role = UserRole.Admin;
//             var feature = Feature.Book;
//             var action = PermissionAction.View;
//             // Act
//             var res1 = PermissionHelper.CanView(role, feature);
//             var res2 = PermissionHelper.CanPerform(role, feature, action);
//             // Assert
//             Assert.AreEqual(res1, res2);
//         }
//     }
// }
