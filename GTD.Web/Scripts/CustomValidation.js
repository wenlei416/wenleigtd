$.validator.addMethod("dategreaterthan", function (value, element, params) {
    //如果2个日期中任意一个为空，就跳过检查
    if (value === "" || $(params).val() === "") {
        return true;
    } 
    return Date.parse(value) >= Date.parse($(params).val());
});

$.validator.unobtrusive.adapters.add("dategreaterthan", ["otherpropertyname"], function (options) {
    options.rules["dategreaterthan"] = "#" + options.params.otherpropertyname;
    options.messages["dategreaterthan"] = options.message;
});