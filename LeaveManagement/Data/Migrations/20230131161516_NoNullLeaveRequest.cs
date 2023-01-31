using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class NoNullLeaveRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RequestEmployeeId",
                table: "LeaveRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "LeaveAllocations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "62b3adbb-e3a7-4f5f-8a05-61d2074df6c2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "63bfe7dc-acab-490d-8ed2-161b850ef7ec", "AQAAAAIAAYagAAAAEKC8zQydcbjymt2mY2MQKI8Ub7lFeVZiq/vDH7L65v6A+z0MEm9rpLsHObqVW5nAew==", "58ebf0a6-00cc-48ea-9145-d86c5f1dae07" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6c2adbb-4f5f-8a05-e3a7-62b3adbb61d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e890c31b-43ff-4833-9bf2-837e9b6d9d77", "AQAAAAIAAYagAAAAEGFffATFqV70WwEuveC8PcsIAZsS3cDrwI2/wQXTVGIWAfhgebTzXfKOI1KIbH9/Ow==", "e0c931be-aab9-4ce2-be9c-e4e01f7c830c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RequestEmployeeId",
                table: "LeaveRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "LeaveAllocations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
    }
}
