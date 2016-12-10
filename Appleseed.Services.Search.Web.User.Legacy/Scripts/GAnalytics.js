(function (i, s, o, g, r, a, m) {
    i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
        (i[r].q = i[r].q || []).push(arguments)
    }, i[r].l = 1 * new Date(); a = s.createElement(o),
    m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
})(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

ga('create', 'UA-70163780-1', 'auto');
ga('send', 'pageview');

function trackTitleforGA() {
    //_gaq.push(['_trackEvent', 'Title', 'News Item List', 'List Item']);
    ga('send', 'event', 'TitleCategory', 'NewsItemList', 'ListItem');
}

function trackViewResourceforGA() {
    //_gaq.push(['_trackEvent', 'View Resource', 'News Item List', 'List Item']);
    ga('send', 'event', 'ViewResourceCategory', 'NewsItemList', 'ListItem');
}

function trackSearchforGA() {
    ga('send', 'event', 'SearchCategory', 'Search', $("#txtQuery").val());
}