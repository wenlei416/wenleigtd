$.validator.addMethod("dategreaterthan", function (value, element, params) {
    return Date.parse(value) >= Date.parse($(params).val());
});

$.validator.unobtrusive.adapters.add("dategreaterthan", ["otherpropertyname"], function (options) {
    options.rules["dategreaterthan"] = "#" + options.params.otherpropertyname;
    options.messages["dategreaterthan"] = options.message;
});