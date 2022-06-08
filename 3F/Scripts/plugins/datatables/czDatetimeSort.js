function formatString(s) {
    return (s.length == 0) ? "00" : (s.length > 1) ? s : "0" + s;
};

jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    "cz_datetime-asc": function (a, b) {
        var x, y;
        if (a == undefined || jQuery.trim(a) != '') {
            var dates = jQuery.trim(a).split(' ');
            var timeString = dates[1].split(':');
            var dateString = dates[0].split('.');

            x = (dateString[2] + formatString(dateString[1]) + formatString(dateString[0]) + formatString(timeString[0]) + formatString(timeString[1]) + formatString(timeString[2]));
        } else {
            x = Infinity; // = l'an 1000 ...
        }

        if (b == undefined || jQuery.trim(b) != '') {
            var dates = jQuery.trim(b).split(' ');
            var timeString = dates[1].split(':');
            var dateString = dates[0].split('.');

            y = (dateString[2] + formatString(dateString[1]) + formatString(dateString[0]) + formatString(timeString[0]) + formatString(timeString[1]) + formatString(timeString[2]));
        } else {
            y = Infinity;
        }
        var z = ((x < y) ? -1 : ((x > y) ? 1 : 0));
        return z;
    },

    "cz_datetime-desc": function (a, b) {
        var x, y;
        if (a == undefined || jQuery.trim(a) != '') {
            var dates = jQuery.trim(a).split(' ');
            var timeString = dates[1].split(':');
            var dateString = dates[0].split('.');
            x = (dateString[2] + formatString(dateString[1]) + formatString(dateString[0]) + formatString(timeString[0]) + formatString(timeString[1]) + formatString(timeString[2]));
        } else {
            x = Infinity;
        }

        if (b == undefined || jQuery.trim(b) != '') {
            var dates = jQuery.trim(b).split(' ');
            var timeString = dates[1].split(':');
            var dateString = dates[0].split('.');
            y = (dateString[2] + formatString(dateString[1]) + formatString(dateString[0]) + formatString(timeString[0]) + formatString(timeString[1]) + formatString(timeString[2]));
        } else {
            y = Infinity;
        }
        var z = ((x < y) ? 1 : ((x > y) ? -1 : 0));
        return z;
    },
});