$(document).ready(function () {
  GetProduct();
});
function GetProduct() {
  $.ajax({
    url: "/Products/GetProductList",
    type: "Get",
    dataType: "json",
    success: OnSuccess,
  });
}
function OnSuccess(response) {
  $("#dataTable").DataTable({
    bProcessing: true,
    lengthChange: true,
    lengthMenu: [
      [5, 10, 25, -1],
      [5, 10, 25, "All"],
    ],
    paginate: true,
    data: response,
    columns: [
      {
        data: "ProductName",
        render: function (data, type, row, meta) {
          return row.productName;
        },
      },
      {
        data: "Price",
        render: function (data, type, row, meta) {
          return row.price;
        },
      },
      {
        data: "Quantity",
        render: function (data, type, row, meta) {
          return row.quantity;
        },
      },
      {
        data: "Description",
        render: function (data, type, row, meta) {
          return row.description;
        },
      },
      {
        data: "",
        render: function (data, type, row, meta) {
          return (
            "<td><a href='/Products/Edit/" +
            row.id +
            "'>Edit</a> | <a href='/Products/Details/" +
            row.id +
            "'>Details</a> | <a href='/Products/Delete/" +
            row.id +
            "'>Delete</a></td>"
          );
        },
      },
    ],
  });
}
