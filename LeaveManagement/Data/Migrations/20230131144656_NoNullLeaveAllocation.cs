using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class NoNullLeaveAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "62b3adbb-e3a7-4f5f-8a05-61d2074df6c2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2f56f6f8-71ab-45f7-9ffa-5c28f65fff9b", "AQAAAAIAAYagAAAAENwZAYYTy0fGjpOSrcSMH1KujEYmLi3VNT4sD9xZw2VXXROiWhZ0p8yc6ST+a9Csfw==", "33840462-0d37-4b79-8987-7a47f735109d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6c2adbb-4f5f-8a05-e3a7-62b3adbb61d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "00a837cb-73fb-4697-b805-b88a5c78311a", "AQAAAAIAAYagAAAAEFTRxUoa1OQ9BvOgu331LnmvgKQgPyYiCUM9y/EbWA8vrWlutneXbru8lIla3kPAPg==", "fba1352e-b761-4f10-9f3e-c93235e22bb2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "62b3adbb-e3a7-4f5f-8a05-61d2074df6c2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "be1e59a7-06b9-42bf-85ed-7afc3d2f9d62", "AQAAAAIAAYagAAAAEEaDIVreeFEiCJZ4dQRcHmOr8rzQLV/G18MBktIet7TGjgecZ1Kwt8FgWWrGLWkgJQ==", "466330e8-1f57-417a-8cb2-b4e5de075309" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6c2adbb-4f5f-8a05-e3a7-62b3adbb61d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "93d00c44-3b17-4b25-9ec4-4052bce9ef44", "AQAAAAIAAYagAAAAEMWwoipGwmDPy5q0kNSVwHjHORTXbNPAhMSoCF2OBj9Ms4H3JeccoKmR7hrvrHancA==", "1814b592-42cc-406c-8c61-9f03f5688a65" });
        }
    }
}
