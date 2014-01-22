/* Custom JS Extensions, Helpers and Utilities
************************************************************/

// Avoid `console` errors in browsers that lack a console.
(function () {
    var method;
    var noop = function () { };
    var methods = [
        'assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'error',
        'exception', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log',
        'markTimeline', 'profile', 'profileEnd', 'table', 'time', 'timeEnd',
        'timeStamp', 'trace', 'warn'
    ];
    var length = methods.length;
    var console = (window.console = window.console || {});

    while (length--) {
        method = methods[length];

        // Only stub undefined methods.
        if (!console[method]) {
            console[method] = noop;
        }
    }
})();



// Array helpers
if (typeof Array.prototype.indexOf !== 'function') {
    Array.prototype.indexOf = function (item, i) {
        if (this == null) throw new TypeError();

        var array = Object(this), length = array.length >>> 0;
        if (length === 0) return -1;

        i = Number(i);
        if (isNaN(i)) {
            i = 0;
        } else if (i !== 0 && isFinite(i)) {
            i = (i > 0 ? 1 : -1) * Math.floor(Math.abs(i));
        }

        if (i > length) return -1;

        var k = i >= 0 ? i : Math.max(length - Math.abs(i), 0);
        for (; k < length; k++)
            if (k in array && array[k] === item) return k;
        return -1;
    }
}
/*if (typeof Array.each !== 'function') {
    Array.prototype.each = function (iterator, context) {
        if (this == null) return;
        if (Array.prototype.forEach && this.forEach === Array.prototype.forEach) {
            this.forEach(iterator, context);
        } else if (this.length === +this.length) {
            for (var i = 0, length = this.length; i < length; i++) {
                if (iterator.call(context, this[i], i, this) === {}) return;
            }
        } else {
            var keys = _.keys(this);
            for (var i = 0, length = keys.length; i < length; i++) {
                if (iterator.call(context, this[keys[i]], keys[i], this) === {}) return;
            }
        }
    };
}*/


// String helpers
if (typeof String.prototype.format !== "function") {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined' ? args[number] : match;
        });
    };
}
if (typeof String.prototype.pad !== "function") {
    String.prototype.pad = function (width, pad) {
        pad = pad || '0';
        var value = this + '';
        return value.length >= width ? value : new Array(width - value.length + 1).join(pad) + value;
    };
}
if (typeof String.prototype.trim !== "function") {
    String.prototype.trim = function () {
        return this.replace(/^\s+|\s+$/g, '');
    };
}
if (typeof String.prototype.contains !== 'function') {
    String.prototype.contains = function (str) {
        return this.indexOf(str) != -1;
    };
}
if (typeof String.prototype.startsWith !== 'function') {
    String.prototype.startsWith = function (str) {
        return this.slice(0, str.length) == str;
    };
}
if (typeof String.prototype.endsWith !== 'function') {
    String.prototype.endsWith = function (str) {
        return this.slice(-str.length) == str;
    };
}
if (typeof String.prototype.replaceAll !== 'function') {
    String.prototype.replaceAll = function (str1, str2, ignore) {
        return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignore ? "gi" : "g")), (typeof (str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
    };
}
if (typeof String.prototype.htmlEncode !== 'function') {
    String.prototype.htmlEncode = function () {
        return this
            .replaceAll('&', '&amp;')
            .replaceAll('"', '&quot;')
            .replaceAll("'", '&#39;')
            .replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;');
    };
}
if (typeof String.prototype.htmlDecode !== 'function') {
    String.prototype.htmlDecode = function () {
        return this
            .replaceAll('&quot;', '"')
            .replaceAll('&#39;', "'")
            .replaceAll('&lt;', '<')
            .replaceAll('&gt;', '>')
            .replaceAll('&amp;', '&');
    };
}



// Date localization helpers
Date.locale = {
    en: {
        month_names: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        month_names_short: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        day_names: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
        day_names_short: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']
    }
};
Date.fromJSON = function (json) {
    return new Date(parseInt(json.substr(6)));
};
Date.prototype.getMonthName = function (lang) {
    lang = lang && (lang in Date.locale) ? lang : 'en';
    return Date.locale[lang].month_names[this.getMonth()];
};
Date.prototype.getMonthNameShort = function (lang) {
    lang = lang && (lang in Date.locale) ? lang : 'en';
    return Date.locale[lang].month_names_short[this.getMonth()];
};
Date.prototype.getDayName = function (lang) {
    lang = lang && (lang in Date.locale) ? lang : 'en';
    return Date.locale[lang].day_names[this.getDay()];
};
Date.prototype.getDayNameShort = function (lang) {
    lang = lang && (lang in Date.locale) ? lang : 'en';
    return Date.locale[lang].day_names_short[this.getDay()];
};
Date.prototype.getDayOrdinal = function () {
    var day = this.getDay();
    return day.toOrdinal();
};



// Number helpers
if (typeof Number.prototype.toOrdinal !== "function") {
    Number.prototype.toOrdinal = function () {
        var n = this % 100;
        var suff = ["th", "st", "nd", "rd", "th"]; // suff for suffix
        var ord = n < 21 ? (n < 4 ? suff[n] : suff[0]) : (n % 10 > 4 ? suff[0] : suff[n % 10]);
        return this + ord;
    };
}
if (typeof Number.prototype.format !== "function") {
    Number.prototype.format = function (delimiter) {
        return this.toString().replace(/\B(?=(\d{3})+(?!\d))/g, delimiter || ",");
    };
}
if (typeof Number.prototype.formatMoney !== "function") {
    Number.prototype.formatMoney = function (c, d, t) {
        var n = this,
            c = isNaN(c = Math.abs(c)) ? 2 : c,
            d = d == undefined ? "." : d,
            t = t == undefined ? "," : t,
            s = n < 0 ? "-" : "",
            i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "",
            j = (j = i.length) > 3 ? j % 3 : 0;
        return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
    };
}
if (typeof Number.prototype.pad !== "function") {
    Number.prototype.pad = function (width, pad) {
        pad = pad || '0';
        var value = this + '';
        return value.length >= width ? value : new Array(width - value.length + 1).join(pad) + value;
    };
}



// Helper that makes specific AJAX calls easier to make
function jsonAjax(request) {
    if (!request.url) {
        console.log('Missing JSON Ajax URL. Request: ', request);
        return;
    }

    return $.ajax({
        url: request.url,
        type: 'POST',
        cache: request.cache || false,
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        data: (request.data) ? JSON.stringify(request.data) : null,
        dataFilter: function (data) {
            return data.d || data;
        },
        beforeSend: function () {
            if (request.beforeSend) request.beforeSend();
        },
        success: function (data) {
            data = data.d || data;
            if (request.success) request.success(data);
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseText);
            if (request.error) request.error(xhr, status, error);
        },
        complete: function () {
            if (request.complete) request.complete();
        }
    });
}
function htmlAjax(request) {
    if (!request.url) {
        console.log('Missing HTML Ajax URL. Request: ', request);
        return;
    }

    return $.ajax({
        url: request.url,
        type: 'POST',
        cache: request.cache || false,
        dataType: 'html',
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        data: (request.data) ? JSON.stringify(request.data) : null,
        dataFilter: function (data) {
            return data.d || data;
        },
        beforeSend: function () {
            if (request.beforeSend) request.beforeSend();
        },
        success: function (data) {
            data = data.d || data;
            if (request.success) request.success(data);
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseText);
            if (request.error) request.error(xhr, status, error);
        },
        complete: function () {
            if (request.complete) request.complete();
        }
    });
}



// jQuery extensions
if (jQuery) {
    $.extend($.expr[':'], {
        broken: function (selector) {
            return $(selector).attr('href') == 'javascript:;';
        }
    });

    // Serialize forms and other objects to JSON
    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name] !== undefined) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };

    // Get the cursor's X and Y coordinates
    $.fn.getCursorPosition = function (context) {
        var el = $(this).get(0);
        var pos = 0;

        // Use the provided context if applicable (say, from an iframe)
        var document = context || document;

        if ('selectionStart' in el) {
            pos = el.selectionStart;
        } else if ('selection' in document) {
            el.focus();
            var Sel = document.selection.createRange();
            var SelLength = document.selection.createRange().text.length;
            Sel.moveStart('character', -el.value.length);
            pos = Sel.text.length - SelLength;
        }
        return pos;
    };
}



// Handlebars helpers and extensions
if (window.Handlebars) {

    // Easily render Handlebars templates
    // Sample: Handlebars.render('#template-wrapper', {});
    Handlebars.render = function (selector, data) {
        var $source = $(selector),
            source = $source.html(),
            $target = $($source.attr('data-target')),
            template = Handlebars.compile(source),
            html = template(data);

        // Confirm that the target exists
        if ($target.length == 0) {

            // Create a new ID for our new target element
            var newTargetID = "{0}-target".format($source.attr('id'));

            // Create the new target, since we didn't have one before
            $target = $('<div id="{0}" />'.format(newTargetID));

            // Insert the new target element and set the target attribute of the template source
            $source.before($target).attr('data-target', newTargetID);
        }

        // Insert the HTML
        $target.html(html);
    };

    // jQuery extension that makes it easier to bind Handlebars templates and data
    jQuery.fn.template = function (data, template, context) {
        var template = Handlebars.compile($(template, context).html());
        var html = template(data);

        $(this).html(html);
    }

    // Common handlebars extensions
    Handlebars.registerHelper('number', function (number, decimals) {
        if (isNaN(decimals)) decimals = 2;
        return number.formatMoney(decimals);
    });
    Handlebars.registerHelper('numberorblank', function (number, decimals) {
        if (isNaN(decimals)) decimals = 2;
        if (number == 0) {
            return '-';
        }
        else {
            return number.formatMoney(decimals);
        }
    });
    Handlebars.registerHelper('intorblank', function (number) {
        if (number == 0) {
            return '-';
        }
        else {
            return number;
        }
    });
    Handlebars.registerHelper('formattednumber', function (number, decimals, delimiter) {
        return Number(number.toFixed(decimals || 0)).format(delimiter);
    });
    Handlebars.registerHelper('money', function (number) {
        return '$' + number.formatMoney(2);
    });
    Handlebars.registerHelper('shortdate', function (text) {
        var date = new Date(text);

        var response = "{0}/{1}/{2}".format(
            date.getMonth() + 1,
            date.getDate(),
            date.getFullYear()
        );

        return response;;
    });
    Handlebars.registerHelper('shortjsondate', function (jsondate) {
        var date = new Date(parseInt(jsondate.substr(6)));

        var response = "{0}/{1}/{2}".format(
            date.getMonth() + 1,
            date.getDate(),
            date.getFullYear()
        );

        return response;;
    });
    Handlebars.registerHelper('longdate', function (text) {
        var date = new Date(text);

        var response = "{0}, {1} {2}, {3}".format(
            date.getDayName(),
            date.getMonthName(),
            date.getDate(),
            date.getFullYear()
        );

        return response;;
    });
    Handlebars.registerHelper('longjsondate', function (jsondate) {
        var date = new Date(parseInt(jsondate.substr(6)));

        var response = "{0}, {1} {2}, {3}".format(
            date.getDayName(),
            date.getMonthName(),
            date.getDate(),
            date.getFullYear()
        );

        return response;;
    });
    Handlebars.registerHelper('fulldate', function (text) {
        var date = new Date(text);
        return date.toLocaleString();
    });
}



// Opening new window popups
window.popup = function (request) {
    // Private functions
    function getOption(option, defaultValue) {
        var result = defaultValue;

        if (typeof option !== "undefined") {
            result = (option == true) ? "yes" : "no";
        }

        return result;
    }

    // Determine the variables
    var width = request.width || 1020,
        height = request.height || 600,
        screenX = typeof window.screenX != 'undefined' ? window.screenX : window.screenLeft,
        screenY = typeof window.screenY != 'undefined' ? window.screenY : window.screenTop,
        outerWidth = typeof window.outerWidth != 'undefined' ? window.outerWidth : document.documentElement.clientWidth,
        outerHeight = typeof window.outerHeight != 'undefined' ? window.outerHeight : (document.documentElement.clientHeight - 22),
        monitorX = (screenX < 0) ? window.screen.width + screenX : screenX,
        left = parseInt(monitorX + ((outerWidth - width) / 2), 10),
        top = parseInt(screenY + ((outerHeight - height) / 3), 10),
        popup = {
            url: request.url || '',
            name: request.name || 'PopupWindow',
            specs: [
                'width={0}'.format(width),
                'height={0}'.format(height),
                'top={0}'.format(top),
                'left={0}'.format(left),
                'location={0}'.format(getOption(request.location, 'no')),
                'directories={0}'.format(getOption(request.directories, 'no')),
                'fullscreen={0}'.format(getOption(request.fullscreen, 'no')),
                'menubar={0}'.format(getOption(request.menubar, 'no')),
                'resizable={0}'.format(getOption(request.resizable, 'yes')),
                'scrollbars={0}'.format(getOption(request.scrollbars, 'yes')),
                'status={0}'.format(getOption(request.status, 'no')),
                'toolbar={0}'.format(getOption(request.toolbar, 'no')),
                'copyhistory={0}'.format(getOption(request.copyhistory, 'no'))
            ].join(','),
            replace: request.replace || true
        };

    // Check for queries
    if (request.query) {
        // Assemble the query
        var query = '';
        var separator = (popup.url.contains('?') ? '&' : '?');
        if (request.query) {
            for (var prop in request.query) {
                query += '{0}{1}={2}'.format(separator, prop, encodeURIComponent(request.query[prop]));
                separator = '&';
            }
        }

        popup.url += query;
    }

    // Open the popup window
    window.open(popup.url, popup.name, popup.specs, popup.replace);
}



// URL helpers (http://blog.stevenlevithan.com/archives/parseuri)
window.Url = window.Url || (function () {
    var settings = {
        strictMode: false,
        key: ["source", "protocol", "authority", "userInfo", "user", "password", "host", "port", "relative", "path", "directory", "file", "query", "anchor"],
        q: {
            name: "queryKey",
            parser: /(?:^|&)([^&=]*)=?([^&]*)/g
        },
        parser: {
            strict: /^(?:([^:\/?#]+):)?(?:\/\/((?:(([^:@]*)(?::([^:@]*))?)?@)?([^:\/?#]*)(?::(\d*))?))?((((?:[^?#\/]*\/)*)([^?#]*))(?:\?([^#]*))?(?:#(.*))?)/,
            loose: /^(?:(?![^:@]+:[^:@\/]*@)([^:\/?#.]+):)?(?:\/\/)?((?:(([^:@]*)(?::([^:@]*))?)?@)?([^:\/?#]*)(?::(\d*))?)(((\/(?:[^?#](?![^?#\/]*\.[^?#\/.]+(?:[?#]|$)))*\/?)?([^?#\/]*))(?:\?([^#]*))?(?:#(.*))?)/
        }
    };

    this.parse = function (url) {
        var o = settings,
            m = o.parser[o.strictMode ? "strict" : "loose"].exec(url),
            uri = {},
            i = 14;

        while (i--) uri[o.key[i]] = m[i] || "";

        uri[o.q.name] = {};
        uri[o.key[12]].replace(o.q.parser, function ($0, $1, $2) {
            if ($1) uri[o.q.name][$1] = $2;
        });

        return uri;
    };
    this.query = function (key, url) {
        return (url) ? this.parse(url).queryKey[key] : this.parse(window.location).queryKey[key];
    }

    // Current URL
    this.current = this.parse(window.location);

    return this;
})();



// Cookie helpers
window.Cookies = (function () {
    this.get = function (name) {
        var dc = document.cookie;
        var prefix = name + "=";
        var begin = dc.indexOf("; " + prefix);
        if (begin == -1) {
            begin = dc.indexOf(prefix);
            if (begin != 0) return null;
        } else {
            begin += 2;
        }
        var end = document.cookie.indexOf(";", begin);
        if (end == -1) {
            end = dc.length;
        }
        return unescape(dc.substring(begin + prefix.length, end));
    };
    this.set = function (name, value, options) {
        document.cookie = name + "=" + escape(value) +
            ((options.expires) ? "; expires=" + options.expires.toUTCString() : "") +
            ((options.path) ? "; path=" + options.path : "") +
            ((options.domain) ? "; domain=" + options.domain : "") +
            ((options.secure) ? "; secure" : "");
    };
    this.clear = function (name, path, domain) {
        if (this.get(name)) {
            document.cookie = name + "=" +
                ((path) ? "; path=" + path : "") +
                ((domain) ? "; domain=" + domain : "") +
                "; expires=Thu, 01-Jan-70 00:00:01 GMT";
        }
    }
    return this;
})();



// Guid object used to make Guids
// Sample: var guid = Guid.newGuid(); // B42A153F-1D9A-4F92-9903-92C11DD684D2 */
window.Guid = window.Guid || (function () {
    this.newGuid = function () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0,
                v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16).toUpperCase();
        });
    }

    return this;
})();



// Type checkers (snippets from UnderscoreJS (http://underscorejs.org/))
window.Type = window.Type || (function () {
    this.isElement = function (obj) {
        return !!(obj && obj.nodeType === 1);
    };
    this.isArray = function (obj) {
        return toString.call(obj) == '[object Array]';
    };
    this.isObject = function (obj) {
        return obj === Object(obj);
    };
    this.isNaN = function (obj) {
        return Type.isNumber(obj) && obj != +obj;
    };
    this.isBoolean = function (obj) {
        return obj === true || obj === false || toString.call(obj) == '[object Boolean]';
    };
    this.isNull = function (obj) {
        return obj === null;
    };
    this.isUndefined = function (obj) {
        return obj === void 0;
    };
    this.isString = function (obj) {
        return toString.call(obj) == '[object String]';
    };
    this.isNumber = function (obj) {
        return toString.call(obj) == '[object Number]';
    };
    this.isDate = function (obj) {
        return toString.call(obj) == '[object Date]';
    };
    this.isRegExp = function (obj) {
        return toString.call(obj) == '[object RegExp]';
    };
    this.isFunction = function (obj) {
        return toString.call(obj) == '[object Function]';
    };
    if (typeof (/./) !== 'function') {
        this.isFunction = function (obj) {
            return typeof obj === 'function';
        };
    }

    return this;
})();



// Browser helper
// Examples:
// var isIe = (Browser.browser == 'Explorer');
// var isVersion9 = (Browser.version = 9);
window.Browser = window.Browser || (function () {
    this.browser;
    this.version;

    this.init = function () {
        this.browser = this.searchString(this.dataBrowser) || "Other";
        this.version = this.searchVersion(navigator.userAgent) || this.searchVersion(navigator.appVersion) || "Unknown";
    };

    this.searchString = function (data) {
        for (var i = 0 ; i < data.length ; i++) {
            var dataString = data[i].string;
            this.versionSearchString = data[i].subString;

            if (dataString.indexOf(data[i].subString) != -1) {
                return data[i].identity;
            }
        }
    };

    this.searchVersion = function (dataString) {
        var index = dataString.indexOf(this.versionSearchString);
        if (index == -1) return;
        return parseFloat(dataString.substring(index + this.versionSearchString.length + 1));
    };

    this.dataBrowser =
        [
            { string: navigator.userAgent, subString: "Chrome", identity: "Chrome" },
            { string: navigator.userAgent, subString: "MSIE", identity: "Explorer" },
            { string: navigator.userAgent, subString: "Firefox", identity: "Firefox" },
            { string: navigator.userAgent, subString: "Safari", identity: "Safari" },
            { string: navigator.userAgent, subString: "Opera", identity: "Opera" }
        ];

    this.init();
    return this;
})();
