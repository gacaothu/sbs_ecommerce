$(".update-profile").click(function () {
    $(".update-form").show();
    $(".general-form").hide()
})
$(".update-shipping-address").click(function () {
    $(".shipping-update").show();
    $(".shipping-form").hide();
    $("#add-shipping").hide();
})
$(".add-shipping").click(function () {
    $("#add-shipping").show();
    $(".shipping-update").hide();
    $("#shippingForm").hide();
})


function callUpdateShipping(addressId) {
    $.ajax({
        url: "/Account/GetShippingAddressById",
        method: "GET",
        data: { id: addressId },
        contentType: "Json",
        success: function (objData) {
            $("#formUpdateShipping #userAddressModel_Uid").val(objData.Uid);
            $("#formUpdateShipping #userAddressModel_Id").val(objData.Id);
            $("#formUpdateShipping #userAddressModel_State").val(objData.State);
            $("#formUpdateShipping #userAddressModel_Address").val(objData.Address);
            $("#formUpdateShipping #userAddressModel_City").val(objData.City);
            $("#formUpdateShipping #userAddressModel_ZipCode").val(objData.ZipCode);
            $("#formUpdateShipping #userAddressModel_Phone").val(objData.Phone);
        },
        error: function (error) {
            alert("error");
        }
    })
}

$(document).ready(function () {
    

})
