var homeAdsCollapsed = false;

function homeAdsAutoCollapse() {
    var container = document.querySelector('.giz-home__body');
    var expander = container.querySelector('.giz-expansion-panel');

    var header = expander.querySelector('.giz-expansion-panel__header');
    header.onclick = function () {
        homeAdsCollapsed = false;
    }

    container.onscroll = function () {
        if (!homeAdsCollapsed) {
            if (expander.classList.contains('expanded')) {
                expansionPanelToggle(expander);
            }
            homeAdsCollapsed = true;
        }
    }
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
            var bbox = popup.getBoundingClientRect();
            if (!isPointWithinRect(event.clientX, event.clientY, bbox.left, bbox.top, bbox.right, bbox.bottom)) {
                //popup.classList.remove('open');

                closePopupEventListenerReferences.forEach((item) => {
                    item.invokeMethodAsync('OnClosePopupEvent', popup.id);
                });
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