function appsSticky() {
    var container = document.querySelector('.giz-apps__body__content');
    if (!container)
        return;

    var sectionHeader = container.querySelector('.giz-section__header');
    if (!sectionHeader)
        return;

    if (sectionHeader.getBoundingClientRect().top - container.getBoundingClientRect().top == 0) {
        sectionHeader.style.visibility = 'hidden';
        container.classList.add('giz-apps__body__content--stuck');

        var parent = container.closest('.giz-apps__body');
        if (!parent)
            return;

        var stuck = parent.querySelector('.giz-section--stuck');
        if (!stuck)
            return;

        stuck.classList.add('visible');
    } else {
        sectionHeader.style.visibility = 'visible';
        container.classList.remove('giz-apps__body__content--stuck');

        var parent = container.closest('.giz-apps__body');
        if (!parent)
            return;

        var stuck = parent.querySelector('.giz-section--stuck');
        if (!stuck)
            return;

        stuck.classList.remove('visible');
    }
}

function registerAppsSticky() {
    var container = document.querySelector('.giz-apps__body__content');
    if (!container)
        return;

    container.addEventListener('scroll', appsSticky);
}

function unregisterAppsSticky() {
    var container = document.querySelector('.giz-apps__body__content');
    if (!container)
        return;

    container.removeEventListener('scroll', appsSticky);
}

var adsCollapsed = false;

function resetAutoHideAds() {
    adsCollapsed = false;
}

function autoHideAds() {
    if (!adsCollapsed) {
        var container = document.querySelector('.giz-ads-container');
        var expander = container.querySelector('.giz-expansion-panel');

        if (expander.classList.contains('expanded')) {
            expansionPanelToggle(expander);
        }
        adsCollapsed = true;
    }
}

function registerAdsAutoCollapse() {
    var container = document.querySelector('.giz-ads-container');
    if (!container)
        return;

    var expander = container.querySelector('.giz-expansion-panel');
    if (!expander)
        return;

    if (expander.classList.contains('expanded'))
        adsCollapsed = false;
    else
        adsCollapsed = true;
    
    var header = expander.querySelector('.giz-expansion-panel__header');
    if (!header)
        return;

    header.addEventListener('click', resetAutoHideAds);
    container.addEventListener('scroll', autoHideAds);
}

function unregisterAdsAutoCollapse() {
    var container = document.querySelector('.giz-home__body');
    if (!container)
        return;

    var expander = container.querySelector('.giz-expansion-panel');
    if (!expander)
        return;

    var header = expander.querySelector('.giz-expansion-panel__header');
    if (!header)
        return;

    header.removeEventListener('click', resetAutoHideAds);
    container.removeEventListener('scroll', autoHideAds);

    resetAutoHideAds();
}

var registeredPopups = [];

function registerPopup(element) {
    registeredPopups.push({
        element: element,
        open: false
    });
}

function unregisterPopup(element) {
    //console.log('unregisterPopup');
    //console.log(element);
}

function isPointWithinRect(x, y, left, top, right, bottom) {
    if (x < left || x > right)
        return false;

    if (y < top || y > bottom)
        return false;

    return true;
}

function closeOpenPopups(event) {
    registeredPopups.forEach(function (value, index, array) {
        const popup = value.element;
        if (popup.classList.contains('open')) {
            var popupContent = popup.querySelector('.giz-dropdown-menu__content');
            if (popupContent) {
                var bbox = popupContent.getBoundingClientRect();
                if (!isPointWithinRect(event.clientX, event.clientY, bbox.left, bbox.top, bbox.right, bbox.bottom)) {
                    //popup.classList.remove('open');

                    closePopupEventListenerReferences.forEach((item) => {
                        item.invokeMethodAsync('OnClosePopupEvent', popup.id);
                    });
                }
            }
        }
    });
}
/*window.onclick = function (event) {
    closeOpenPopups from here does not work
}*/
var closePopupEventListenerReferences = [];
function addClosePopupEventListener(objRef) {
    closePopupEventListenerReferences.push(objRef);
}
function removeClosePopupEventEventListener(objRef) {
    var index = findElementIndexById(closePopupEventListenerReferences, objRef);
    if (index > -1) {
        closePopupEventListenerReferences.splice(index, 1);
    }
}


var registeredTabs = [];

function getRegisteredTab(element) {
    for (var i = 0; i < registeredTabs.length; i++) {
        if (registeredTabs[i].element == element)
            return registeredTabs[i];
    }
}

function registerTab(element) {
    var registeredTab = getRegisteredTab(element);

    if (!registeredTab) {
        registeredTab = {
            element: element,
            scroll: 0
        };
        registeredTabs.push(registeredTab);
    }

    var wrapper = element.querySelector('.giz-client-tab__wrapper');
    var content = wrapper.querySelector('.giz-client-tab__content');

    var wrapperBox = wrapper.getBoundingClientRect();
    var contentBox = content.getBoundingClientRect();

    var previous = element.querySelector('.giz-client-tab__previous');
    var next = element.querySelector('.giz-client-tab__next');

    if (content.scrollWidth <= wrapperBox.width) {
        previous.style.display = 'none';
        next.style.display = 'none';
    } else {
        previous.disabled = true;
        next.disabled = false;
    }

    return registeredTab;
}

function unregisterTab(element) {
}

function tabScrollPrevious(element) {
    var registeredTab = getRegisteredTab(element);
    if (!registeredTab) {
        registeredTab = registerTab(element);
    }

    var wrapper = element.querySelector('.giz-client-tab__wrapper');
    var content = wrapper.querySelector('.giz-client-tab__content');

    var wrapperBox = wrapper.getBoundingClientRect();
    var contentBox = content.getBoundingClientRect();

    var previousScroll = registeredTab.scroll;

    var previous = element.querySelector('.giz-client-tab__previous');
    var next = element.querySelector('.giz-client-tab__next');

    next.disabled = false;
    var nextScroll = previousScroll - wrapperBox.width;
    if (nextScroll < 0) {
        nextScroll = 0;

        previous.disabled = true;
    } else {
        previous.disabled = false;
    }

    registeredTab.scroll = nextScroll;
    content.style.marginLeft = "-" + nextScroll + "px";
}

function tabScrollNext(element) {
    var registeredTab = getRegisteredTab(element);
    if (!registeredTab) {
        registeredTab = registerTab(element);
    }

    var wrapper = element.querySelector('.giz-client-tab__wrapper');
    var content = wrapper.querySelector('.giz-client-tab__content');

    var wrapperBox = wrapper.getBoundingClientRect();
    var contentBox = content.getBoundingClientRect();

    var previousScroll = registeredTab.scroll;

    var previous = element.querySelector('.giz-client-tab__previous');
    var next = element.querySelector('.giz-client-tab__next');

    previous.disabled = false;
    var nextScroll = previousScroll + wrapperBox.width;
    if (nextScroll >= content.scrollWidth - wrapperBox.width) {
        nextScroll = content.scrollWidth - wrapperBox.width;

        next.disabled = true;
    } else {
        next.disabled = false;
    }

    registeredTab.scroll = nextScroll;
    content.style.marginLeft = "-" + nextScroll + "px";
}

function tabItemBringIntoView(element) {
    var tab = element.parentElement.parentElement.parentElement;
    var registeredTab = getRegisteredTab(tab);
    if (!registeredTab) {
        registeredTab = registerTab(tab);
    }

    var elementBox = element.getBoundingClientRect();
    var content = element.parentElement;
    var contentBox = content.getBoundingClientRect();
    var wrapper = content.parentElement;
    var wrapperBox = wrapper.getBoundingClientRect();

    var previous = tab.querySelector('.giz-client-tab__previous');
    var next = tab.querySelector('.giz-client-tab__next');

    if (elementBox.left < wrapperBox.left) {
        var previousScroll = registeredTab.scroll;

        next.disabled = false;
        var nextScroll = previousScroll - (wrapperBox.left - elementBox.left);
        if (nextScroll <= 0) {
            nextScroll = 0;

            previous.disabled = true;
        } else {
            previous.disabled = false;
        }

        registeredTab.scroll = nextScroll;
        content.style.marginLeft = "-" + nextScroll + "px";
    }

    if (elementBox.right > contentBox.right) {
        var previousScroll = registeredTab.scroll;

        previous.disabled = false;
        var nextScroll = previousScroll + (elementBox.right - contentBox.right);
        if (nextScroll > content.scrollWidth - wrapperBox.width) {
            nextScroll = content.scrollWidth - wrapperBox.width;

            next.disabled = true;
        } else {
            next.disabled = false;
        }

        registeredTab.scroll = nextScroll;
        content.style.marginLeft = "-" + nextScroll + "px";
    }
}

var expandingOperations = [];

function expandElement(element) {
    if (element) {
        if (expandingOperations[element]) {
            clearTimeout(expandingOperations[element]);
            expandingOperations[element] = null;
        }

        if (element.classList.contains('collapsing')) {
            element.classList.remove('collapsing');
        }

        element.classList.add('open');

        var height = element.getBoundingClientRect().height;

        element.style.setProperty('--abh', height + 'px');
        element.classList.add('expanding');

        var expandingTimeout = setTimeout(function () {
            element.classList.remove('expanding');
            expandingOperations[element] = null;
        }, 500, element);

        expandingOperations[element] = expandingTimeout;
    }
}

function collapseElement(element) {
    if (element) {
        if (expandingOperations[element]) {
            clearTimeout(expandingOperations[element]);
            expandingOperations[element] = null;
        }

        if (element.classList.contains('open')) {
            if (element.classList.contains('expanding')) {
                element.classList.remove('expanding');
            }
            var height = element.getBoundingClientRect().height;

            element.style.setProperty('--abh', height + 'px');
            element.classList.add('collapsing');

            var expandingTimeout = setTimeout(function () {
                element.classList.remove('open');
                element.classList.remove('collapsing');
                expandingOperations[element] = null;
            }, 500, element);

            expandingOperations[element] = expandingTimeout;
        }
    }
}

/*window.addEventListener("load", (event) => {
    console.log("page is fully loaded");
});

window.addEventListener("mousedown", (event) => {
    console.log(event.button);

    if (event.button == 3 || event.button == 4) {
        event.preventDefault();
        event.stopImmediatePropagation();
        event.stopPropagation();
        console.log("try");
    }
});*/

function navigationGoBack() {
    window.history.back();
}

function navigationBlock() {
    window.addEventListener('unload', function () {
        console.log("unload");
        console.log(document.URL);
    });

    window.addEventListener('beforeunload', function () {
        console.log("beforeunload");
        console.log(document.URL);
    });

    window.addEventListener('pagehide', function () {
        console.log("pagehide");
        console.log(document.URL);
    });

    window.addEventListener('load', function () {
        console.log("load");
        console.log(document.URL);
    });

    window.addEventListener('pushState', function () {
        console.log("pushState");
        console.log(document.URL);
    });

    window.addEventListener('replaceState', function () {
        console.log("replaceState");
        console.log(document.URL);
    });

    window.addEventListener('popstate', function () {
        console.log("popstate");
        console.log(document.URL);

        if (document.URL.endsWith("/"))
            window.history.go(1);

        if (document.URL.endsWith("/registrationindex"))
            window.history.go(-1);
    });
}

/*function test() {
    window.location.replace("https://localhost:5001/home");
}*/


function productDetailsFitHostGroups(element) {
    try {
        if (element) {
            var additional = element.querySelector('.giz-time-product-host-group--additional');
            if (additional) {
                var elementr = element.getBoundingClientRect();
                if (element.scrollWidth > elementr.width) {
                    var additionalr = additional.getBoundingClientRect();
                    var items = element.querySelectorAll('.giz-time-product-host-group.dynamic');
                    var total = additionalr.width;
                    var hidden = 0;

                    for (var i = 0; i < items.length; i++) {
                        if (total > elementr.width) {
                            items[i].style = 'display: none';
                            hidden += 1;
                        } else {
                            var hideAdditional = false;

                            if (hidden == 0 && i == items.length - 1) {
                                //If this is the last item check if we can fit it in.
                                if (elementr.width > (total - additionalr.width + items[i].getBoundingClientRect().width)) {
                                    hideAdditional = true;
                                }
                            }

                            if (hideAdditional) {
                                additional.style = 'display: none';
                            }
                            else {
                                total += items[i].getBoundingClientRect().width + 8; //TODO: AAA
                                if (total > elementr.width) {
                                    items[i].style = 'display: none';
                                    hidden += 1;
                                }
                            }
                        }
                    }

                    if (hidden > 0) {
                        additional.innerHTML = "+" + hidden.toString();
                    }
                } else {
                    additional.style = 'display: none';
                }
            }
        }
    } catch (error) {
        console.error(error);
    }
}