$(document).ready(function () {
    'use strict';
    
    /*----------------------------------------
    Smooth Scrolling
    ----------------------------------------*/
    $('a[href*="#"]:not([href="#"])').on('click', function () {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
            var target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
            if (target.length) {
                $('html, body').animate({
                    scrollTop: target.offset().top
                }, 1000);
                return false;
            }
        }
    });
    
    /*----------------------------------------
    Top selections
    ----------------------------------------*/
    $('.top-block').on('click', function () {
        $(this).children('.top-sub').slideToggle();
    });
    
    $('.nav-tabs a').on('click', function (e) {
        e.preventDefault();
        $(this).tab('show');
    });
    
    /*----------------------------------------
    Mobile Navigation
    ----------------------------------------*/
    function mobileMenuToggle() {
        
        if ($(window).width() < 992) {
            $('#main-nav').css('max-height', $(window).height() - 240);
            $('#main-nav > .nav > .menu-item').on('click', function () {
                $('.sub-menu').stop().slideUp();
                $(this).children('.sub-menu').stop().slideToggle('5000');
            });
        }
        
        /*----------------------------------------
        Mobile Others collapse
        ----------------------------------------*/
        if ($(window).width() < 768) {
            $('.accounts-block').on('click', function () {
                $(this).children('#account-menu').stop().slideToggle('5000');
            });

            $('.toggle-btn').on('click', function () {
                $(this).toggleClass('close');
                $(this).parent('.module-title, .widget-title').siblings('.mobile-collapse').slideToggle();
            });
        }
    }
    
    mobileMenuToggle();
    
    /*----------------------------------------
    Default Carousel
    ----------------------------------------*/
    $(".default-carousel").owlCarousel({
        margin: 30,
        nav: true,
        dots: false,
        lazyLoad: true,
        navText: ["<i class=\"fa fa-angle-left\"></i>", "<i class=\"fa fa-angle-right\"></i>"],
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 2
            },
            1200: {
                items: 3
            }
        }
    });
    
    /*----------------------------------------
    Default carousel for 1 item
    ----------------------------------------*/
    $('.carousel-item-1').owlCarousel({
        items: 1,
        nav: true,
        dots: false,
        lazyLoad: true,
        navText: ["<i class=\"fa fa-angle-left\"></i>", "<i class=\"fa fa-angle-right\"></i>"]
    });
    
    /*----------------------------------------
    Default carousel for 4 items
    ----------------------------------------*/
    $('.carousel-item-4').owlCarousel({
        margin: 30,
        nav: true,
        dots: false,
        lazyLoad: true,
        navText: ["<i class=\"fa fa-angle-left\"></i>", "<i class=\"fa fa-angle-right\"></i>"],
        responsive: {
            0: {
                items: 1
            },
            768: {
                items: 2
            },
            992: {
                items: 3
            },
            1200: {
                items: 4
            }
        }
    });
    
    /*----------------------------------------
    Default Image Carousel
    ----------------------------------------*/
    $(".default-img-carousel").owlCarousel({
        items: 1,
        dots: true,
        loop: true,
        lazyLoad: true,
        autoplay: true,
        autoplaySpeed: 1000,
        autoplayTimeout: 3000
    });

    /*----------------------------------------
    Hero Slider
    ----------------------------------------*/
    $("#hero-slider").owlCarousel({
        items: 1,
        loop: true,
        autoplay: true,
        autoplaySpeed: 1000,
        autoplayTimeout: 10000,
        lazyLoad: true
    });
    
    /*----------------------------------------
    Testimonial Carousel
    ----------------------------------------*/
    $("#testimonials-carousel").owlCarousel({
        items: 1,
        nav: true,
        dots: false,
        loop: true,
        autoplay: true,
        autoplaySpeed: 1000,
        autoplayTimeout: 5000,
        lazyLoad: true,
        navText: ["<i class=\"fa fa-angle-left\"></i>", "<i class=\"fa fa-angle-right\"></i>"]
    });
    
    /*----------------------------------------
    Article Carousel
    ----------------------------------------*/
    $(".article-carousel").owlCarousel({
        margin: 30,
        nav: true,
        dots: false,
        lazyLoad: true,
        navText: ["<i class=\"fa fa-angle-left\"></i>", "<i class=\"fa fa-angle-right\"></i>"],
        responsive: {
            0: {
                items: 1
            },
            768: {
                items: 2
            },
            992: {
                items: 3
            },
            1200: {
                items: 2
            }
        }
    });
    
    /*----------------------------------------
    Brand Carousel
    ----------------------------------------*/
    $(".brand-carousel").owlCarousel({
        margin: 30,
        nav: true,
        dots: false,
        autoplay: true,
        loop: true,
        lazyLoad: true,
        navText: ["<i class=\"fa fa-angle-left\"></i>", "<i class=\"fa fa-angle-right\"></i>"],
        responsive: {
            0: {
                items: 1
            },
            480: {
                items: 2
            },
            600: {
                items: 3
            },
            768: {
                items: 3
            },
            992: {
                items: 4
            },
            1200: {
                items: 5
            }
        }
    });
    
    /*----------------------------------------
    Offer Countdown
    ----------------------------------------*/
    $('.item-countdown-time').each(function () {
        var endTime = $(this).data('time');
        $(this).countdown(endTime, function (tm) {
            $(this).html(tm.strftime('<span class="section_count"><span class="tcount days">%D </span><span class="text">Days</span></span><span class="section_count"><span class="tcount hours">%H</span><span class="text">Hours</span></span><span class="section_count"><span class="tcount minutes">%M</span><span class="text">Mins</span></span><span class="section_count"><span class="tcount seconds">%S</span><span class="text">Secs</span></span>'));
        });
    });
    
    
    /*----------------------------------------
    Back to Top
    ----------------------------------------*/
    $(window).on('scroll', function () {
        if ($(this).scrollTop() >= 200) {
            $('#toTop').addClass('totop-active');
        } else {
            $('#toTop').removeClass('totop-active');
        }
    });
    
    /*----------------------------------------
    WooCommerce input form handing
    ----------------------------------------*/
    $('.view-mode').on('click', function (e) {
        e.preventDefault();
        $('.view-mode').removeClass('active');
        $(this).addClass('active');
        $('#mode_switcher').removeClass();
        $('#mode_switcher').addClass($(this).data('mode'));
    });
    
    $('.input-rule select').on('change', function () {
        $(this).siblings('.input-style').text($(this).children('option:selected').text());
    });
    
    $('.input-rule input[type=checkbox]').on('change', function () {
        if ($(this).is(':checked')) {
            $(this).parent().addClass('selected');
        } else {
            $(this).parent().removeClass('selected');
        }
    });
    
    $('.input-rule input[type=radio]').on('change', function () {
        if ($(this).is(':checked')) {
            $(this).parent().siblings().removeClass('selected');
            $(this).parent().addClass('selected');
        }
    });
    
    $('input[name=color]').on('click', function () {
        $(this).toggleClass('on');
    });
    
    $('.ui-ranger').each(function () {
        var slideDiv = $(this);
        $(this).slider({
            range: true,
            min: slideDiv.data('min'),
            max: slideDiv.data('max'),
            step: 0.01,
            values: [slideDiv.data('min'), slideDiv.data('max')],
            slide: function (event, ui) {
                $(this).siblings('input').val('$' + ui.values[0] + ' - $' + ui.values[1]);
            }
        });
        $(this).siblings('input').val("$" + $(this).slider("values", 0) + ".00 - $" + $(this).slider("values", 1) + ".00");
    });
    
    //Fancybox quickview
    $('.to-view').each(function () {
        var w = $(this).data('width');
        $(this).fancybox({
            'hideOnContentClick': true,
            'width': w,
            'autoSize': false
        });
    });
    
    //Image Zoom
    function zooming(el) {
        el.elevateZoom({
            lenszoom: false,
            scrollZoom: true,
            lensShape: 'round',
            lensBorder: 'none',
            lensColour: 'transparent',
            tint: true,
            zoomWindowWidth: 336,
            zoomWindowHeight: 336,
            borderColour: 'rgb(136, 136, 136)',
            borderSize: 1
        });
    }
    
    zooming($('.zoomable'));
    
    function handleZoom() {
        if ($(window).width() > 991) {
           // $('.zoomable').data('elevateZoom').changeState('enable');
        } else {
          //  $('.zoomable').data('elevateZoom').changeState('disable');
        }
    }
	
	if ($(".zoomable")[0]){
		handleZoom();
	}
    
     $('.list-thumb').on('mouseover', function () {
        var holder = $('#img-holder');
        var img = $(this).data('image');
        var imgZoom = $(this).data('image-zoom');
        
        holder.attr('src', img);
        holder.data('zoom-image', imgZoom);
        $('.list-thumb').removeClass('active');
        $(this).addClass('active');
        zooming(holder);
    });
    
    //Rating/review form
    $('.star-rating a').on('click', function () {
        $(this).prevAll().andSelf().addClass('rate-in');
        $(this).nextAll().removeClass('rate-in');
        $(this).parent().siblings('input').val($(this).data('rate'));
    }).on('mouseover', function () {
        $(this).prevAll().andSelf().css('color', '#eb5858');
        $(this).nextAll().css('color', '#dedede');
    });
    
    $('.zero-rating').on('click', function () {
        $(this).siblings('input').val(0);
        $(this).siblings('.star-rating').children('a').removeClass('rate-in').css('color', '#dedede');
    });
    
    //Animate ProgressBar
	
    function animateProgressBar(pb) {
        if ($.fn.visible && $(pb).visible() && !$(pb).hasClass('animated')) {
            $(pb).css('width', $(pb).attr('aria-valuenow') + '%');
            $(pb).addClass('animated');
        }
    }
    
    function initProgressBar() {
        var progressBar = $('.progress-bar');
        progressBar.each(function () {
            animateProgressBar(this);
        });
    }
    
    initProgressBar();
    
    
    //Window events
    $(window).on('scroll', function () {
        initProgressBar();
    });
    
    $(window).on('resize orientationchange', function () {
        mobileMenuToggle();
        handleZoom();
    });
    
});