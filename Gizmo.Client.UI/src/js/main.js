var adsCollapsed = false;

function resetAutoHideAds() {
    adsCollapsed = false;
}

function autoHideAds() {
    var container = document.querySelector('.giz-ads-container');
    var expander = container.querySelector('.giz-expansion-panel');

    if (!adsCollapsed) {
        if (expander.classList.contains('expanded')) {
            expansionPanelToggle(expander);
        }
        adsCollapsed = true;
    }
}

function registerAdsAutoCollapse() {
    var container = document.querySelector('.giz-ads-container');
    var expander = container.querySelector('.giz-expansion-panel');

    if (expander.classList.contains('expanded'))
        adsCollapsed = false;
    else
        adsCollapsed = true;
    
    var header = expander.querySelector('.giz-expansion-panel__header');

    header.addEventListener('click', resetAutoHideAds);
    container.addEventListener('scroll', autoHideAds);
}

function unregisterAdsAutoCollapse() {
    var container = document.querySelector('.giz-home__body');
    var expander = container.querySelector('.giz-expansion-panel');
    var header = expander.querySelector('.giz-expansion-panel__header');

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