;
(function ($, window, document, undefined) {
    // 定义widget
    // "ui" 是namespace
    // "autocompleteTrigger" 是widge的名字
    $.widget("ui.autocompleteTrigger", {

        //options的默认值
        options: {
            triggerStart: "%{",
            triggerEnd: "}"
        },

        // 构造函数
        _create: function () {
            this.triggered = false;
            console.log("this " + this.element);
            //this.element是指使用这个widget的对象
            //这里是给这个对象加上autocomplete的widget，并发进去一个参数$extend
            //应该是用这个extend参数中的几个函数，替代了原来autocomplete的几个函数
            this.element.autocomplete($.extend({
                search: function () {
                    var acTrigger =
                        $(this).data("autocompleteTrigger") || $(this).data("uiAutocompleteTrigger");
                    //console.log(acTrigger);
                    return acTrigger.triggered;
                },
                select: function (event, ui) {
                    /**
                     * @description if a item is selected, insert the value between triggerStart and triggerEnd
                     */
                    var acTrigger =
                        $(this).data("autocompleteTrigger") || $(this).data("uiAutocompleteTrigger");
                    var trigger = acTrigger.options.triggerStart;
                    var cursorPosition = acTrigger.getCursorPosition();

                    if ($(this).is('input,textarea')) {
                        var text = $(this).val();
                        var lastTriggerPosition = text.substring(0, cursorPosition).lastIndexOf(trigger);


                        var triggerStartSecond = " #";
                        var lastTriggerStartSecondPosition = text.substring(0, cursorPosition).lastIndexOf(triggerStartSecond);

                        if (lastTriggerPosition >= 0 && lastTriggerStartSecondPosition < 0) {
                        }

                        if (lastTriggerPosition < 0 && lastTriggerStartSecondPosition >= 0) {
                            lastTriggerPosition = lastTriggerStartSecondPosition;
                        }

                        if (lastTriggerPosition >= 0 && lastTriggerStartSecondPosition >= 0 && lastTriggerPosition < lastTriggerStartSecondPosition) {
                            lastTriggerPosition = lastTriggerStartSecondPosition;
                        }

                        var firstTextPart = text.substring(0, lastTriggerPosition + trigger.length) +
                            ui.item.value +
                            acTrigger.options.triggerEnd;
                        $(this).val(firstTextPart + text.substring(cursorPosition, text.length));
                        acTrigger.setCursorPosition(firstTextPart.length);
                    } else {

                        var text = $(this).text();
                        var html = $(this).html();

                        var searchTerm = text.substring(0, cursorPosition);

                        var i = 0;
                        var index = 0;
                        while (i < searchTerm.length) {
                            index = html.lastIndexOf(searchTerm.substring(i));
                            if (index !== -1) {
                                break;
                            }
                            i++;
                        }

                        var htmlCursorPosition = index + searchTerm.substring(i).length;
                        var htmlLastTriggerPosition =
                            html.substring(0, htmlCursorPosition).lastIndexOf(trigger);
                        var htmlFirstTextPart = html.substring(0, htmlLastTriggerPosition + trigger.length) +
                            ui.item.value +
                            acTrigger.options.triggerEnd;

                        var lastTriggerPosition = text.substring(0, cursorPosition).lastIndexOf(trigger);
                        var firstTextPart = text.substring(0, lastTriggerPosition + trigger.length) +
                            ui.item.value +
                            acTrigger.options.triggerEnd;

                        $(this).html(htmlFirstTextPart + html.substring(htmlCursorPosition, html.length));
                        acTrigger.setCursorPosition(firstTextPart.length);
                    }

                    acTrigger.triggered = false;
                    return false;
                },
                focus: function () {
                    /**
                     * @description prevent to replace the hole text, if a item is hovered
                     */

                    return false;
                },
                minLength: 0
            },       this.options))//这个this.options 相当于把参数传递给autocomplete了？
                //然后再个这个对象绑定一个keyupd的事件
                .bind("keyup",
                    function (event) {
                        /**
                         * @description Bind to keyup-events to detect text changes.
                         * If the trigger is found before the cursor, autocomplete will be called
                         */
                        var widget = $(this);
                        var acTrigger = $(this).data("autocompleteTrigger") || $(this).data("uiAutocompleteTrigger");
                        var delay = typeof acTrigger.options.delay === "undefined" ? 0 : acTrigger.options.delay;

                        if (event.keyCode !== $.ui.keyCode.UP && event.keyCode !== $.ui.keyCode.DOWN) {
                            var text;
                            if ($(this).is("input,textarea")) {
                                text = $(this).val();
                            } else {
                                text = $(this).text();
                            }
                            acTrigger.textValue = text;
                            if (typeof acTrigger.locked === "undefined") {
                                acTrigger.locked = false;
                            }

                            if (!acTrigger.locked) {
                                acTrigger.locked = true;
                                acTrigger.timeout = setTimeout(function () {
                                    acTrigger.launchAutocomplete(acTrigger, widget);
                                },
                                    delay);
                            }
                        }

                    });
        },

        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        },

        getCursorPosition: function () {
            var elem = this.element[0];
            return jQuery(elem).caret();
        },

        setCursorPosition: function (position) {
            var elem = this.element[0];
            return jQuery(elem).caret(position);
        },

        launchAutocomplete: function (acTrigger, widget) {
            acTrigger.locked = false;
            var text = acTrigger.textValue;
            var textLength = text.length;
            var cursorPosition = acTrigger.getCursorPosition();
            var query;
            var lastTriggerPosition;
            var trigger = acTrigger.options.triggerStart;
            var triggerEnd = acTrigger.options.triggerEnd;

            lastTriggerPosition = text.substring(0, cursorPosition).lastIndexOf(trigger);
            var lastTriggerEndPosition = text.substring(0, cursorPosition).lastIndexOf(triggerEnd);
            var triggerSecond = " #";
            var lastTriggerStartSecondPosition = text.substring(0, cursorPosition).lastIndexOf(triggerSecond);

            var laetsource = acTrigger.options.sourceA;

            if (lastTriggerPosition >= 0 && lastTriggerStartSecondPosition < 0) {
                laetsource = acTrigger.options.sourceA;
            }

            if (lastTriggerPosition < 0 && lastTriggerStartSecondPosition >= 0) {
                lastTriggerPosition = lastTriggerStartSecondPosition;
                laetsource = acTrigger.options.sourceB;

            }

            if (lastTriggerPosition >= 0 && lastTriggerStartSecondPosition >= 0 && lastTriggerPosition <= lastTriggerStartSecondPosition) {
                lastTriggerPosition = lastTriggerStartSecondPosition;
                laetsource = acTrigger.options.sourceB;

            }
            //console.log("lastTriggerPosition: " + lastTriggerPosition);

            if ((lastTriggerEndPosition < lastTriggerPosition || textLength >= trigger.length) && lastTriggerPosition !== -1) {
                query = text.substring(lastTriggerPosition + trigger.length, cursorPosition);
                //console.log(query);
                acTrigger.triggered = true;
                widget.autocomplete({
                    source: laetsource
                });
                widget.autocomplete("search", query);
            } else {
                acTrigger.triggered = false;
                widget.autocomplete("close");
            }
        }
    });
})(jQuery, window, document);