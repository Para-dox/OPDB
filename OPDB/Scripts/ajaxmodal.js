var UIModals = function () {

    var initModals = function() {

        $.fn.modalmanager.defaults.resize = true;
        $.fn.modalmanager.defaults.spinner = '<div class="loading-spinner fade" style="width: 200px; margin-left: -100px;"><img  align="middle">&nbsp;<span style="font-weight:300; color: #eee; font-size: 18px; font-family:Open Sans;">&nbsp;Loading...</div>';


        var $modal = $('#ajax-modal');

        $('.modal_ajax_btn').unbind('click').on('click', function() {
         
            var self = this;
            var cssClass = $(this).attr('data-css');
            $modal.removeClass('notes');
            $modal.removeClass('patient');
            $modal.removeClass('document');
            $modal.removeClass('reminder');
            $modal.addClass(cssClass);
            var title = "Modal";
            if ($(self).attr("data-title")) {
                title = $(self).attr("data-title");
            }
            $('body').modalmanager('loading');
            var mode = 'POST';
            if ($(self).attr('data-mode')) {
                mode = $(self).attr('data-mode');
            }
           
                var url = $(self).attr('data-url');
                if ($(self).attr('data-id') && $(self).attr('data-id') != '') {
                    url = url.replace('-1', $(self).attr('data-id'));
                }
                if (mode == 'POST') {
                    $.post(url, function(data) {
                       
                        $('#ajax-modal .modal-body').html(data);
                        if ($(self).attr('data-ajax') && $(self).attr('data-ajax') == '1') {
                            $("#btn_savenote").addClass('update');
                            } else {
                            $("#btn_savenote").removeClass('update');
                        }
                        $('#ajax-modal .modal-header').find('h3').html(title);
                        $('#ajax-modal').modal();
                       
                        if (typeof (BindEdit) == "function") {
                            BindEdit();
                        }
                    });
                } else {
                    $.get(url, function (data) {
                       
                        $('#ajax-modal .modal-body').html(data);
                        if ($(self).attr('data-ajax')) {
                            $("#btn_savenote").addClass('update');
                        } else {
                            $("#btn_savenote").removeClass('update');
                        }
                        $('#ajax-modal .modal-header').find('h3').html(title);
                        $modal.modal();
                        if (typeof (BindEdit) == "function") {
                            BindEdit();
                        }
                    });
                }

            return false;
           
        });

        $('#ajax-modal').unbind('click').on('click', '.update', function () {
            alert('shit');
            //debugger;
            App.blockUI(".update");
            $modal.modal('loading');
         
            var url = $(this).attr('data-url');
            var formId = $(this).attr('data-form');
            
            $(this).unbind('click');
            $(this).attr('href', '');
            var ajaxRequest = $.ajax({
                url: url,
                dataType: 'html',
                type: 'post',
                data: $(formId).serialize()
            });
            ajaxRequest.done(function (data) {
                
                $modal.modal('loading');
                if (data.length > 1000) {
                    
                    $('#ajax-modal .modal-body').html(data);
                    $('#ajax-modal').find('.loading-mask').removeClass('.loading-mask');
                    $('.validation-summary-errors').addClass('alert-error');
                } else {
                    if (window.location.href.indexOf('Script') == -1) {
                        window.location.href = data;
                    }
                    $modal.modal('hide');
                   
                }
            });
            ajaxRequest.error(function(data) {
                alert(data);
            });
            


           
            App.unblockUI(".update");
            return false;
        });
    

    };

    return {
       
        init: function () {
            initModals();
        }

    };

}();