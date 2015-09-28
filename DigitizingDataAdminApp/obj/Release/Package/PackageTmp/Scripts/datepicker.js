$(function () {
    $.validator.addMethod('date',
    function (value, element) {
        if (this.optional(element)) {
            return true;
        }
        var ok = true;
        try {
            $.datepicker.parseDate('yy/mm/dd', value);
        }
        catch (err) {
            ok = false;
        }
        return ok;
    });
    $(".datefield").datepicker({ dateFormat: 'yy/mm/dd', changeYear: true });
});