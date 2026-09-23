window.InternalFunctions = class InternalFunctions {
  static dotnetObjectReference;

  static SetDotnetObjectReference(value) {
    this.dotnetObjectReference = value;
  }

  static FullScreen = class FullScreen {
    /**
     * Subscribes to browser full screen change event.
     * @param {string} callbackName callBack function name.
     */
    static async SubscribeOnFullScreenChange(callbackName) {
      try {
        this.subscribe(callbackName);
      } catch (error) {
        await InternalFunctions.dotnetObjectReference.invokeMethodAsync(
          callbackName,
          false,
          error.message
        );
      }
    }

    /**
     * Unsubscribes from browser full screen change event.
     * @param {string} callbackName callBack function name.
     */
    static async UnsubscribeOnFullScreenChange(callbackName) {
      try {
        this.unsubscribe(callbackName);
      } catch (error) {
        await InternalFunctions.dotnetObjectReference.invokeMethodAsync(
          callbackName,
          false,
          error.message
        );
      }
    }

    static subscribe(callbackName) {
      const listener = (_) => this.fullScreenChangeHandler(callbackName);

      window.addEventListener("fullscreenchange", listener);
      window.addEventListener("mozfullscreenchange", listener);
      window.addEventListener("webkitfullscreenchange", listener);
      window.addEventListener("msfullscreenchange", listener);
    }

    static unsubscribe(callbackName) {
      const listener = (_) => this.fullScreenChangeHandler(callbackName);

      window.removeEventListener("fullscreenchange", listener);
      window.removeEventListener("mozfullscreenchange", listener);
      window.removeEventListener("webkitfullscreenchange", listener);
      window.removeEventListener("msfullscreenchange", listener);
    }

    /**
     * Handles full screen mode change events.
     * @param {string} callbackName - The name of the method to be called when the full screen mode is changed.
     */
    static async fullScreenChangeHandler(callbackName) {
      try {
        let isFullScreen =
          document.fullscreenElement ||
          document.mozFullScreenElement ||
          document.webkitFullscreenElement ||
          document.msFullscreenElement;

        isFullScreen = !!isFullScreen;

        await InternalFunctions.dotnetObjectReference.invokeMethodAsync(
          callbackName,
          isFullScreen,
          null
        );
      } catch (error) {
        await InternalFunctions.dotnetObjectReference.invokeMethodAsync(
          callbackName,
          false,
          error.message
        );
      }
    }
  };
};

window.ClientFullScreen = window.appsSticky = function appsSticky() {
  var container = document.querySelector(".giz-apps__body__content");
  if (!container) return;

  var sectionHeader = container.querySelector(".giz-section__header");
  if (!sectionHeader) return;

  if (
    sectionHeader.getBoundingClientRect().top -
      container.getBoundingClientRect().top ==
    0
  ) {
    sectionHeader.style.visibility = "hidden";
    container.classList.add("giz-apps__body__content--stuck");

    var parent = container.closest(".giz-apps__body");
    if (!parent) return;

    var stuck = parent.querySelector(".giz-section--stuck");
    if (!stuck) return;

    stuck.classList.add("visible");
  } else {
    sectionHeader.style.visibility = "visible";
    container.classList.remove("giz-apps__body__content--stuck");

    var parent = container.closest(".giz-apps__body");
    if (!parent) return;

    var stuck = parent.querySelector(".giz-section--stuck");
    if (!stuck) return;

    stuck.classList.remove("visible");
  }
};

// The handler measures two rects and then writes classes, which is a forced layout on
// every scroll event - dozens per second on a long application list. Coalesced onto one
// animation frame so it runs once per painted frame at most, and registered passive so
// the compositor never waits on it before scrolling.
var appsStickyFrame = null;

function appsStickyScroll() {
  if (appsStickyFrame !== null) return;

  appsStickyFrame = requestAnimationFrame(function () {
    appsStickyFrame = null;
    appsSticky();
  });
}

window.registerAppsSticky = function registerAppsSticky() {
  var container = document.querySelector(".giz-apps__body__content");
  if (!container) return;

  container.addEventListener("scroll", appsStickyScroll, { passive: true });
};

window.unregisterAppsSticky = function unregisterAppsSticky() {
  if (appsStickyFrame !== null) {
    cancelAnimationFrame(appsStickyFrame);
    appsStickyFrame = null;
  }

  var container = document.querySelector(".giz-apps__body__content");
  if (!container) return;

  container.removeEventListener("scroll", appsStickyScroll);
};

var adsCollapsed = false;

window.resetAutoHideAds = function resetAutoHideAds() {
  adsCollapsed = false;
};

window.autoHideAds = function autoHideAds() {
  if (!adsCollapsed) {
    var container = document.querySelector(".giz-ads-container");
    var expander = container.querySelector(".giz-expansion-panel");

    if (expander.classList.contains("expanded")) {
      expansionPanelToggle(expander);
    }
    adsCollapsed = true;
  }
};

window.registerAdsAutoCollapse = function registerAdsAutoCollapse() {
  var container = document.querySelector(".giz-ads-container");
  if (!container) return;

  var expander = container.querySelector(".giz-expansion-panel");
  if (!expander) return;

  if (expander.classList.contains("expanded")) adsCollapsed = false;
  else adsCollapsed = true;

  var header = expander.querySelector(".giz-expansion-panel__header");
  if (!header) return;

  header.addEventListener("click", resetAutoHideAds);
  container.addEventListener("scroll", autoHideAds, { passive: true });
};

window.unregisterAdsAutoCollapse = function unregisterAdsAutoCollapse() {
  var container = document.querySelector(".giz-home__body");
  if (!container) return;

  var expander = container.querySelector(".giz-expansion-panel");
  if (!expander) return;

  var header = expander.querySelector(".giz-expansion-panel__header");
  if (!header) return;

  header.removeEventListener("click", resetAutoHideAds);
  container.removeEventListener("scroll", autoHideAds);

  resetAutoHideAds();
};

//Popups

var registeredPopups = [];

window.registerPopup = function registerPopup(element) {
  registeredPopups.push({
    element: element,
    open: false,
  });
  //console.log('registerPopup');
  //console.log(element);
};

window.unregisterPopup = function unregisterPopup(element) {
    var objRefIndex = -1;

    registeredPopups.forEach((item, index) => {
        if (item.element.id == element.id)
            objRefIndex = index;
    });

    if (objRefIndex > -1) {
        registeredVideoComponents.splice(objRefIndex, 1);

        //console.log('unregisterPopup');
        //console.log(element);
    }
};

window.isPointWithinRect = function isPointWithinRect(
  x,
  y,
  left,
  top,
  right,
  bottom
) {
  if (x < left || x > right) return false;

  if (y < top || y > bottom) return false;

  return true;
};

window.closeOpenPopups = function closeOpenPopups(event) {
    registeredPopups.forEach(function (value, index, array) {
        const popup = value.element;
        if (popup.classList.contains("open")) {
            var popupContent;
            if (popup.classList.contains("giz-client-popup")) {
                popupContent = popup;
            } else {
                popupContent = popup.querySelector(".giz-dropdown-menu__content");
            }
            if (popupContent) {
                var bbox = popupContent.getBoundingClientRect();
                if (
                    !isPointWithinRect(
                        event.clientX,
                        event.clientY,
                        bbox.left,
                        bbox.top,
                        bbox.right,
                        bbox.bottom
                    )
                ) {
                    //popup.classList.remove('open');

                    closePopupEventListenerReferences.forEach((item) => {
                        item.invokeMethodAsync("OnClosePopupEvent", popup.id);
                    });
                }
            }
        }
    });
};
/*window.onclick = function (event) {
    closeOpenPopups from here does not work
}*/
var closePopupEventListenerReferences = [];
window.addClosePopupEventListener = function addClosePopupEventListener(
  objRef
) {
  closePopupEventListenerReferences.push(objRef);
};

window.removeClosePopupEventEventListener =
  function removeClosePopupEventEventListener(objRef) {
    var index = findElementIndexById(closePopupEventListenerReferences, objRef);
    if (index > -1) {
      closePopupEventListenerReferences.splice(index, 1);
    }
  };

//End Popups

var registeredTabs = [];

window.getRegisteredTab = function getRegisteredTab(element) {
  for (var i = 0; i < registeredTabs.length; i++) {
    if (registeredTabs[i].element == element) return registeredTabs[i];
  }
};

window.registerTab = function registerTab(element) {
  var registeredTab = getRegisteredTab(element);

  if (!registeredTab) {
    registeredTab = {
      element: element,
      scroll: 0,
    };
    registeredTabs.push(registeredTab);
  }

  var wrapper = element.querySelector(".giz-client-tab__wrapper");
  var content = wrapper.querySelector(".giz-client-tab__content");

  var wrapperBox = wrapper.getBoundingClientRect();
  var contentBox = content.getBoundingClientRect();

  var previous = element.querySelector(".giz-client-tab__previous");
  var next = element.querySelector(".giz-client-tab__next");

  if (content.scrollWidth <= wrapperBox.width) {
    previous.style.display = "none";
    next.style.display = "none";
  } else {
    previous.disabled = true;
    next.disabled = false;
  }

  return registeredTab;
};

window.unregisterTab = function unregisterTab(element) {};

window.tabScrollPrevious = function tabScrollPrevious(element) {
  var registeredTab = getRegisteredTab(element);
  if (!registeredTab) {
    registeredTab = registerTab(element);
  }

  var wrapper = element.querySelector(".giz-client-tab__wrapper");
  var content = wrapper.querySelector(".giz-client-tab__content");

  var wrapperBox = wrapper.getBoundingClientRect();
  var contentBox = content.getBoundingClientRect();

  var previousScroll = registeredTab.scroll;

  var previous = element.querySelector(".giz-client-tab__previous");
  var next = element.querySelector(".giz-client-tab__next");

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
};

window.tabScrollNext = function tabScrollNext(element) {
  var registeredTab = getRegisteredTab(element);
  if (!registeredTab) {
    registeredTab = registerTab(element);
  }

  var wrapper = element.querySelector(".giz-client-tab__wrapper");
  var content = wrapper.querySelector(".giz-client-tab__content");

  var wrapperBox = wrapper.getBoundingClientRect();
  var contentBox = content.getBoundingClientRect();

  var previousScroll = registeredTab.scroll;

  var previous = element.querySelector(".giz-client-tab__previous");
  var next = element.querySelector(".giz-client-tab__next");

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
};

window.tabItemBringIntoView = function tabItemBringIntoView(element) {
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

  var previous = tab.querySelector(".giz-client-tab__previous");
  var next = tab.querySelector(".giz-client-tab__next");

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
};

var expandingOperations = [];

window.expandElement = function expandElement(element) {
  if (element) {
    if (expandingOperations[element]) {
      clearTimeout(expandingOperations[element]);
      expandingOperations[element] = null;
    }

    if (element.classList.contains("collapsing")) {
      element.classList.remove("collapsing");
    }

    element.classList.add("open");

    var height = element.getBoundingClientRect().height;

    element.style.setProperty("--abh", height + "px");
    element.classList.add("expanding");

    var expandingTimeout = setTimeout(
      function () {
        element.classList.remove("expanding");
        expandingOperations[element] = null;
      },
      500,
      element
    );

    expandingOperations[element] = expandingTimeout;
  }
};

window.collapseElement = function collapseElement(element) {
  if (element) {
    if (expandingOperations[element]) {
      clearTimeout(expandingOperations[element]);
      expandingOperations[element] = null;
    }

    if (element.classList.contains("open")) {
      if (element.classList.contains("expanding")) {
        element.classList.remove("expanding");
      }
      var height = element.getBoundingClientRect().height;

      element.style.setProperty("--abh", height + "px");
      element.classList.add("collapsing");

      var expandingTimeout = setTimeout(
        function () {
          element.classList.remove("open");
          element.classList.remove("collapsing");
          expandingOperations[element] = null;
        },
        500,
        element
      );

      expandingOperations[element] = expandingTimeout;
    }
  }
};

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

window.navigationGoBack = function navigationGoBack() {
  window.history.back();
};

window.navigationBlock = function navigationBlock() {
  window.addEventListener("unload", function () {
    console.log("unload");
    console.log(document.URL);
  });

  window.addEventListener("beforeunload", function () {
    console.log("beforeunload");
    console.log(document.URL);
  });

  window.addEventListener("pagehide", function () {
    console.log("pagehide");
    console.log(document.URL);
  });

  window.addEventListener("load", function () {
    console.log("load");
    console.log(document.URL);
  });

  window.addEventListener("pushState", function () {
    console.log("pushState");
    console.log(document.URL);
  });

  window.addEventListener("replaceState", function () {
    console.log("replaceState");
    console.log(document.URL);
  });

  window.addEventListener("popstate", function () {
    console.log("popstate");
    console.log(document.URL);

    if (document.URL.endsWith("/")) window.history.go(1);

    if (document.URL.endsWith("/registrationindex")) window.history.go(-1);
  });
};

/*function test() {
    window.location.replace("https://localhost:5001/home");
}*/

window.productDetailsFitHostGroups = function productDetailsFitHostGroups(
  element
) {
  try {
    if (element) {
      var additional = element.querySelector(
        ".giz-time-product-host-group--additional"
      );
      if (additional) {
        var items = element.querySelectorAll(
          ".giz-time-product-host-group.dynamic"
        );

        for (var i = 0; i < items.length; i++) {
          items[i].style = "display: block";
        }

        additional.style = "display: block";
        additional.innerHTML = "+99";

        var elementr = element.getBoundingClientRect();
        if (element.scrollWidth > elementr.width) {
          var additionalr = additional.getBoundingClientRect();

          var gap = 8;

          if (items.length > 0) {
            if (items.length > 1) {
              gap =
                items[1].getBoundingClientRect().left -
                items[0].getBoundingClientRect().right;
            } else {
              gap = additionalr.left - items[0].getBoundingClientRect().right;
            }
          }

          var total = additionalr.width;
          var hidden = 0;

          for (var i = 0; i < items.length; i++) {
            if (total > elementr.width) {
              items[i].style = "display: none";
              hidden += 1;
            } else {
              items[i].style = "display: block";
              var hideAdditional = false;

              if (hidden == 0 && i == items.length - 1) {
                //If this is the last item check if we can fit it in.
                if (
                  elementr.width >
                  total -
                    additionalr.width +
                    items[i].getBoundingClientRect().width
                ) {
                  hideAdditional = true;
                }
              }

              if (hideAdditional) {
                additional.style = "display: none";
              } else {
                total += items[i].getBoundingClientRect().width + gap;
                if (total > elementr.width) {
                  items[i].style = "display: none";
                  hidden += 1;
                }
              }
            }
          }

          if (hidden > 0) {
            additional.style = "display: block";
            additional.innerHTML = "+" + hidden.toString();
          } else {
            additional.style = "display: none";
          }
        } else {
          additional.style = "display: none";
        }
      }
    }
  } catch (error) {
    console.error(error);
  }
};

window.userTimeProductsFitHostGroups = function userTimeProductsFitHostGroups(
    element
) {
    try {
        if (element) {
            var additional = element.querySelector(
                ".giz-user-time-products-host-group--additional"
            );
            if (additional) {
                var items = element.querySelectorAll(
                    ".giz-user-time-products-host-group.dynamic"
                );

                for (var i = 0; i < items.length; i++) {
                    items[i].style = "display: block";
                }

                additional.style = "display: block";
                additional.innerHTML = "+99";

                var elementr = element.getBoundingClientRect();
                if (element.scrollWidth > elementr.width) {
                    var additionalr = additional.getBoundingClientRect();

                    var gap = 8;

                    if (items.length > 0) {
                        if (items.length > 1) {
                            gap =
                                items[1].getBoundingClientRect().left -
                                items[0].getBoundingClientRect().right;
                        } else {
                            gap = additionalr.left - items[0].getBoundingClientRect().right;
                        }
                    }

                    var total = additionalr.width;
                    var hidden = 0;

                    for (var i = 0; i < items.length; i++) {
                        if (total > elementr.width) {
                            items[i].style = "display: none";
                            hidden += 1;
                        } else {
                            items[i].style = "display: block";
                            var hideAdditional = false;

                            if (hidden == 0 && i == items.length - 1) {
                                //If this is the last item check if we can fit it in.
                                if (
                                    elementr.width >
                                    total -
                                    additionalr.width +
                                    items[i].getBoundingClientRect().width
                                ) {
                                    hideAdditional = true;
                                }
                            }

                            if (hideAdditional) {
                                additional.style = "display: none";
                            } else {
                                total += items[i].getBoundingClientRect().width + gap;
                                if (total > elementr.width) {
                                    items[i].style = "display: none";
                                    hidden += 1;
                                }
                            }
                        }
                    }

                    if (hidden > 0) {
                        additional.style = "display: block";
                        additional.innerHTML = "+" + hidden.toString();
                    } else {
                        additional.style = "display: none";
                    }
                } else {
                    additional.style = "display: none";
                }
            }
        }
    } catch (error) {
        console.error(error);
    }
};

window.setNotificationHeight = function setNotificationHeight(item) {
    var element = document.querySelector('[data-id="' + item.toString() + '"]');
    if (element) {
        //console.log(element);
        var height = element.getBoundingClientRect().height;
        //console.log(height);
        element.style.setProperty("--notification-height", height + "px");
        //console.log(element);
        return height;
    } else {
        //console.log("Not found!");
        return 0;
    }
};

window.setNotificationsContainerHeight = function setNotificationsContainerHeight(element) {
    if (element) {
        var height = element.getBoundingClientRect().height;
        //console.log(height);
        element.style.setProperty("--notifications-container-height", -1 * height + "px");
        return height;
    } else {
        console.error("null object parameter");
        return 0;
    }
};

var animationEventListenerReferences = [];

window.addAnimationEventListener = function addAnimationEventListener(objRef) {
  animationEventListenerReferences.push(objRef);
};

window.removeAnimationEventListener = function removeAnimationEventListener(
  objRef
) {
  var index = findElementIndexById(animationEventListenerReferences, objRef);
  if (index > -1) {
    animationEventListenerReferences.splice(index, 1);
  }
};

window.onAnimationEvent = function onAnimationEvent(event, state) {
  //console.log(event);
  animationEventListenerReferences.forEach((item) => {
    var id = event.target.id;
    var animation = event.animationName;
    if (!id) {
      id = event.target.dataset.id;
    }
    item.invokeMethodAsync("OnAnimationEvent", {
      Id: id,
      AnimationState: state,
      AnimationName: animation,
    });
  });
};

window.onAnimationStartEvent = function onAnimationStartEvent(event) {
  onAnimationEvent(event, 0);
};

window.onAnimationIterationEvent = function onAnimationIterationEvent(event) {
  onAnimationEvent(event, 1);
};

window.onAnimationEndEvent = function onAnimationEndEvent(event) {
  onAnimationEvent(event, 2);
};

window.onAnimationCancelEvent = function onAnimationCancelEvent(event) {
  onAnimationEvent(event, 3);
};

var registeredAnimatedComponents = [];

window.registerAnimatedComponent = function registerAnimatedComponent(element) {
  if (element) {
    registeredAnimatedComponents.push({
      element: element,
      open: false,
    });

    element.addEventListener("animationstart", onAnimationStartEvent);

    element.addEventListener("animationiteration", onAnimationIterationEvent);

    element.addEventListener("animationend", onAnimationEndEvent);

    element.addEventListener("animationcancel", onAnimationCancelEvent);
  }
};

window.unregisterAnimatedComponent = function unregisterAnimatedComponent(
  element
) {
  //console.log('unregisterAnimatedComponent');
  //console.log(element);
};

window.getFontSize = function getFontSize() {
    var style = window.getComputedStyle(document.documentElement, null).getPropertyValue('font-size');
    var fontSize = parseFloat(style);
    return fontSize;
};

var videoEventListenerReferences = [];

window.addVideoEventListener = function addVideoEventListener(objRef) {
    videoEventListenerReferences.push(objRef);
};

window.removeVideoEventListener = function removeVideoEventListener(
    objRef
) {
    var index = findElementIndexById(videoEventListenerReferences, objRef);
    if (index > -1) {
        videoEventListenerReferences.splice(index, 1);
    }
};

window.onVideoEvent = function onVideoEvent(event, state) {
    videoEventListenerReferences.forEach((item) => {
        var id = event.target.id;
        item.invokeMethodAsync("OnVideoEvent", {
            Id: id,
            VideoState: state,
        });
    });
};

window.onVideoCanPlayThroughEvent = function onVideoCanPlayThroughEvent(event) {
    onVideoEvent(event, 0);
    //console.log('onVideoCanPlayThroughEvent');
    //console.log(event);
    //try {
    //    event.target.play();
    //} catch (error) {
    //    console.error(error);
    //}
};

window.onVideoEndedEvent = function onVideoEndedEvent(event) {
    onVideoEvent(event, 1);
    //console.log('onVideoEndedEvent');
    //console.log(event);
};

window.videoEvent = function videoEvent(event) {
    //console.log('onVideoEvent');
    //console.log(event.type);
    if (event.type == 'pause') {
        //console.log(event);
        if (event.target.error) {
            //console.log(event.target.currentTime + ' of ' + event.target.duration);
            //console.log(event.target.error);
            onVideoEvent(event, 2);
        }
    }
    //if (event.type == 'timeupdate')
        //console.log(event.target.currentTime + ' of ' + event.target.duration);
};

var registeredVideoComponents = [];

window.registerVideoComponent = function registerVideoComponent(element) {
    if (element) {
        registeredVideoComponents.push({
            element: element,
            open: false,
        });

        //element.addEventListener("playing", videoEvent);
        //element.addEventListener("waiting", videoEvent);
        //element.addEventListener("seeking", videoEvent);
        //element.addEventListener("seeked", videoEvent);
        //element.addEventListener("loadedmetadata", videoEvent);
        //element.addEventListener("loadeddata", videoEvent);
        //element.addEventListener("canplay", videoEvent);
        //element.addEventListener("durationchange", videoEvent);
        //element.addEventListener("timeupdate", videoEvent);
       // element.addEventListener("play", videoEvent);
        element.addEventListener("pause", videoEvent);
        //element.addEventListener("ratechange", videoEvent);
        //element.addEventListener("volumechange", videoEvent);
        //element.addEventListener("suspend", videoEvent);
        //element.addEventListener("emptied", videoEvent);
        //element.addEventListener("stalled", videoEvent);

        element.addEventListener("canplaythrough", onVideoCanPlayThroughEvent);
        element.addEventListener("ended", onVideoEndedEvent);
    }
};

window.unregisterVideoComponent = function unregisterVideoComponent(element) {
    var objRefIndex = -1;

    registeredVideoComponents.forEach((item, index) => {
        if (item.element.id == element.id)
            objRefIndex = index;
    });

    if (objRefIndex > -1) {
        element.removeEventListener("pause", videoEvent);
        element.removeEventListener("canplaythrough", onVideoCanPlayThroughEvent);
        element.removeEventListener("ended", onVideoEndedEvent);

        registeredVideoComponents.splice(objRefIndex, 1);

        //console.log('unregisterVideoComponent');
        //console.log(element);
    }
};

window.playVideo = function playVideo(id) {
    try {
        //console.log('element.play()');
        var element = document.getElementById(id);
        if (element.currentTime > 0) {
            element.currentTime = 0;
        } else {
            element.muted = true;
            element.play();
        }
    } catch (error) {
        console.error(error);
    }
};

window.resetVideo = function resetVideo(id) {
    try {
        //console.log('reset()');
        var element = document.getElementById(id);
        //element.currentTime = 0;
        element.load();
    } catch (error) {
        console.error(error);
    }
};

//
window.getWindowSize = function getWindowSize() {
    return {
        width: window.innerWidth,
        height: window.innerHeight,
    };
};
//
window.getElementBoundingClientRect = function getElementBoundingClientRect(element) {
    if (element) {
        return element.getBoundingClientRect();
    } else {
        console.log("Cannot read getBoundingClientRect of null element.");
        return {
            "x": 0,
            "y": 0,
            "width": 0,
            "height": 0,
            "top": 0,
            "right": 0,
            "bottom": 0,
            "left": 0
        };
    }
};
//
window.scrollListItemIntoView = function scrollListItemIntoView(element) {
    if (element) {
        let parent = element.parentElement;

        let { top: eTop } = element.getBoundingClientRect();
        let { top: pTop } = parent.getBoundingClientRect();

        parent.parentElement.parentElement.scrollTop = eTop - pTop;
    }
};
//
/*window.scrollDatePickerYear = function scrollDatePickerYear() {
  var items = document.getElementsByClassName('giz-date-picker-year-count active');

  for (var i = 0; i < items.length; i++) {
    items[i].scrollIntoView({ block: 'center' });
  }
};*/
//
window.focusElement = function focusElement(element) {
    element.focus();
};
//
window.setPropByElement = function setPropByElement(element, property, value) {
    element[property] = value;
};
//
/*window.scrollItemToTop = function scrollItemToTop(element) {
  if (element) {
    let parent = element.parentElement;

    let { top: eTop } = element.getBoundingClientRect();
    let { top: pTop } = parent.getBoundingClientRect();

    parent.scrollTop = eTop - pTop;
  }
};*/
//
/*window.getElementScrollSize = function getElementScrollSize(element) {
  if (element) {
    let parent = element.parentElement;

    return {
      width: parent.scrollLeft,
      height: parent.scrollTop,
    };
  }

  return {
    width: 0,
    height: 0,
  };
};*/
//
window.findElementIndexById = function findElementIndexById(list, elementRef) {
    var objRefIndex = -1;

    list.forEach((item, index) => {
        if (item._id == elementRef._id) objRefIndex = index;
    });

    return objRefIndex;
};
//
var globalResizeEventListener;
var globalResizeEventListenerReferences = [];
window.addWindowResizeEventListener = function addWindowResizeEventListener(elementRef) {
    if (!globalResizeEventListener) {
        globalResizeEventListener = window.addEventListener('resize', windowResizeHandler);
    }

    globalResizeEventListenerReferences.push(elementRef);
};
window.removeWindowResizeEventListener = function removeWindowResizeEventListener(elementRef) {
    //not working var index = globalResizeEventListenerReferences.indexOf(elementRef);
    var index = findElementIndexById(globalResizeEventListenerReferences, elementRef);
    if (index > -1) {
        globalResizeEventListenerReferences.splice(index, 1);
    }
};
window.windowResizeHandler = function windowResizeHandler(event) {
    globalResizeEventListenerReferences.forEach(item => {
        item.invokeMethodAsync('OnWindowResizeEvent', { width: window.innerWidth, height: window.innerHeight });
    });
};
//
var globalMouseDownEventListener;
var globalMouseDownEventListenerReferences = [];
window.addWindowMouseDownEventListener = function addWindowMouseDownEventListener(elementRef) {
    if (!globalMouseDownEventListener) {
        globalMouseDownEventListener = window.addEventListener('mousedown', windowMouseDownHandler);
    }

    globalMouseDownEventListenerReferences.push(elementRef);
};
window.removeWindowMouseDownEventListener = function removeWindowMouseDownEventListener(elementRef) {
    //not working var index = globalMouseDownEventListenerReferences.indexOf(elementRef);
    var index = findElementIndexById(globalMouseDownEventListenerReferences, elementRef);
    if (index > -1) {
        globalMouseDownEventListenerReferences.splice(index, 1);
    }
};
window.windowMouseDownHandler = function windowMouseDownHandler(event) {
    globalMouseDownEventListenerReferences.forEach(item => {
        item.invokeMethodAsync('OnWindowMouseDownEvent', { clientX: event.clientX, clientY: event.clientY });
    });
};
//
window.writeLine = function writeLine(message) {
    window.console.log(message);
};

var expansionPanelOperations = [];

window.expansionPanelToggle = function expansionPanelToggle(elementRef) {
    if (elementRef) {
        var expander = elementRef;

        if (expansionPanelOperations[expander]) return;

        if (!expander.classList.contains('expanded')) {
            expander.classList.add('expanded');
            var body = expander.querySelector('.giz-expansion-panel__body');
            var height = body.getBoundingClientRect().height;

            body.style.setProperty('--abh', height + 'px');
            expander.classList.add('expanding');

            var expansionPanelTimeout = setTimeout(
                function () {
                    expander.classList.remove('expanding');
                    expansionPanelOperations[expander] = null;

                    expansionPanelEventListenerReferences.forEach(item => {
                        item.invokeMethodAsync('OnExpansionPanelEvent', { Id: expander.id, IsCollapsed: false });
                    });
                },
                500,
                expander,
            );

            expansionPanelOperations[expander] = expansionPanelTimeout;
        } else {
            var body = expander.querySelector('.giz-expansion-panel__body');
            var height = body.getBoundingClientRect().height;

            body.style.setProperty('--abh', height + 'px');
            expander.classList.add('collapsing');

            var expansionPanelTimeout = setTimeout(
                function () {
                    expander.classList.remove('expanded');
                    expander.classList.remove('collapsing');
                    expansionPanelOperations[expander] = null;

                    expansionPanelEventListenerReferences.forEach(item => {
                        item.invokeMethodAsync('OnExpansionPanelEvent', { Id: expander.id, IsCollapsed: true });
                    });
                },
                500,
                expander,
            );
        }
    }
};
//
/*window.dropSelection = function dropSelection(element) {
  if (element) {
    element.setSelectionRange(100, 100);
  }
};*/
//
window.getInputSelectionRange = function getInputSelectionRange(element) {
    if (element) {
        return {
            selectionStart: element.selectionStart,
            selectionEnd: element.selectionEnd,
        };
    }
};
//
window.setInputSelectionAll = function setInputSelectionAll(element) {
    if (element) {
        element.select();
    }
};
//
window.setInputCaretIndex = function setInputCaretIndex(element, index) {
    if (element) {
        if (element.createTextRange) {
            var range = element.createTextRange();
            range.move('character', index);
            range.select();
        } else {
            element.focus();
            element.setSelectionRange(index, index);
        }
    }
};
//
const focusableElementsSelector = 'input:not([disabled]), textarea:not([disabled]), select:not([disabled]), button:not([disabled]), a[href]:not([disabled]), [tabindex = "0"]';
//
window.focusPrevious = function focusPrevious(element) {
    if (element) {
        var inputs = document.querySelectorAll(focusableElementsSelector);
        if (inputs.length > 0) {
            for (var i = 1; i < inputs.length; i++) {
                if (inputs[i] == element) {
                    //TODO: A
                    inputs[i - 1].focus();
                    return;
                }
            }
            //If not found already and the selector is correct then the element is the first in the page. Start over again.
            inputs[inputs.length - 1].focus();
        }
    }
};
//
window.focusNext = function focusNext(element) {
    //console.log(element);
    if (element) {
        var inputs = document.querySelectorAll(focusableElementsSelector);
        if (inputs.length > 0) {
            for (var i = 0; i < inputs.length - 1; i++) {
                if (inputs[i] == element) {
                    //TODO: A
                    inputs[i + 1].focus();
                    //console.log(inputs[i + 1]);
                    return;
                }
            }
            //If not found already and the selector is correct then the element is the last in the page. Start over again.
            inputs[0].focus();
            //console.log("start");
        }
    }
};
//
window.focusTrapCheck = function focusTrapCheck(e) {
    if (!(e.key == 'Tab' || e.keyCode == 9)) return;

    var dialog = e.target.closest('.giz-dialog');

    if (dialog) {
        var inputs = dialog.querySelectorAll(focusableElementsSelector);

        if (e.shiftKey) {
            if (document.activeElement == inputs[0]) {
                inputs[inputs.length - 1].focus();
                e.preventDefault();
            }
        } else {
            if (document.activeElement == inputs[inputs.length - 1]) {
                inputs[0].focus();
                e.preventDefault();
            }
        }
    }
};
//
window.focusTrap = function focusTrap(element) {
    if (element) {
        element.addEventListener('keydown', focusTrapCheck);
        element.focus();
    }
};
//
window.focusUntrap = function focusUntrap(element) {
    if (element) {
        element.removeEventListener('keydown', focusTrapCheck);
    }
};

var registeredExpansionPanels = [];

window.registerExpansionPanel = function registerExpansionPanel(elementRef, isCollapsed) {
    registeredExpansionPanels.push({
        element: elementRef,
        isCollapsed: isCollapsed,
    });
};

window.unregisterExpansionPanel = function unregisterExpansionPanel(elementRef) {
    var objRefIndex = -1;

    registeredExpansionPanels.forEach((item, index) => {
        if (item.element.id == elementRef.id) objRefIndex = index;
    });

    if (objRefIndex > -1) {
        expansionPanelEventListenerReferences.splice(objRefIndex, 1);
    }
};

var expansionPanelEventListenerReferences = [];

window.addExpansionPanelEventListener = function addExpansionPanelEventListener(elementRef) {
    expansionPanelEventListenerReferences.push(elementRef);
};

window.removeExpansionPanelEventListener = function removeExpansionPanelEventListener(elementRef) {
    var index = findElementIndexById(expansionPanelEventListenerReferences, elementRef);
    if (index > -1) {
        expansionPanelEventListenerReferences.splice(index, 1);
    }
};

// ───────────────────────────── avatar editor ─────────────────────────────
// Only ever used when the club turns pictures on (--gg-avatars) and opens the
// editor; nothing here runs otherwise. The framing is done here rather than in
// Blazor because a pointermove per frame has no business crossing interop.

var _avatarPasteHandler = null;

window.setupAvatarPaste = function setupAvatarPaste(dotNetRef, callbackName) {
    window.teardownAvatarPaste();

    _avatarPasteHandler = function (event) {
        const items = (event.clipboardData || window.clipboardData || {}).items;
        if (!items) return;

        for (const item of items) {
            if (item.type && item.type.startsWith("image/")) {
                const blob = item.getAsFile();
                const reader = new FileReader();
                reader.onload = () => {
                    dotNetRef.invokeMethodAsync(callbackName, reader.result);
                };
                reader.readAsDataURL(blob);
                event.preventDefault();
                break;
            }
        }
    };

    document.addEventListener("paste", _avatarPasteHandler);
};

window.teardownAvatarPaste = function teardownAvatarPaste() {
    if (_avatarPasteHandler) {
        document.removeEventListener("paste", _avatarPasteHandler);
        _avatarPasteHandler = null;
    }
};

//=============== Avatar crop editor =================//
// Раньше картинка резалась сразу и вслепую: бралась центральная квадратная
// область и сжималась в 512px. Для портрета в полный рост это означало
// «аватарка — живот». Теперь пользователь сам выбирает область: тянет
// картинку и меняет масштаб, а в круг попадает ровно то, что видно.
//
// Геометрия. Сцена квадратная со стороной S, круг вписан, диаметр D = S.
// Картинка натуральных размеров nw*nh лежит по центру сцены и двигается
// трансформом translate(tx,ty) scale(s). Тогда точка изображения под центром
// круга это (nw/2 - tx/s, nh/2 - ty/s), а диаметр круга в пикселях исходника
// равен D/s — из этого и считается прямоугольник для canvas при экспорте.
//
// Минимальный масштаб — тот, при котором круг ещё полностью закрыт картинкой:
// s >= D / min(nw, nh). Смещение всегда зажимается так, чтобы за краем круга
// не оказалось пустоты, поэтому «дырок» в аватарке не бывает в принципе.

var _avatarCrop = null;

function _avatarCropClamp() {
    const c = _avatarCrop;
    if (!c) return;

    const maxX = Math.max(0, (c.nw * c.scale - c.d) / 2);
    const maxY = Math.max(0, (c.nh * c.scale - c.d) / 2);

    c.tx = Math.min(maxX, Math.max(-maxX, c.tx));
    c.ty = Math.min(maxY, Math.max(-maxY, c.ty));
}

function _avatarCropApply() {
    const c = _avatarCrop;
    if (!c) return;

    _avatarCropClamp();
    c.img.style.transform =
        "translate(-50%, -50%) translate(" + c.tx + "px, " + c.ty + "px) scale(" + c.scale + ")";
}

window.avatarCropInit = function avatarCropInit(stage, dataUrl) {
    window.avatarCropDispose();

    return new Promise((resolve, reject) => {
        if (!stage) {
            reject(new Error("No crop stage element."));
            return;
        }

        const img = new Image();
        img.onload = () => {
            const d = Math.min(stage.clientWidth, stage.clientHeight);
            const nw = img.naturalWidth;
            const nh = img.naturalHeight;
            const base = d / Math.min(nw, nh);

            img.className = "giz-avatar-crop__img";
            img.draggable = false;
            img.style.width = nw + "px";
            img.style.height = nh + "px";

            stage.appendChild(img);

            _avatarCrop = {
                stage: stage, img: img, nw: nw, nh: nh, d: d,
                base: base,
                scale: base,
                tx: 0, ty: 0,
                dragging: false,
                lastX: 0, lastY: 0,
                pointerId: null,
            };

            const onDown = (e) => {
                const c = _avatarCrop;
                if (!c) return;
                c.dragging = true;
                c.pointerId = e.pointerId;
                c.lastX = e.clientX;
                c.lastY = e.clientY;
                stage.setPointerCapture(e.pointerId);
                stage.classList.add("is-dragging");
            };

            const onMove = (e) => {
                const c = _avatarCrop;
                if (!c || !c.dragging || e.pointerId !== c.pointerId) return;
                c.tx += e.clientX - c.lastX;
                c.ty += e.clientY - c.lastY;
                c.lastX = e.clientX;
                c.lastY = e.clientY;
                _avatarCropApply();
            };

            const onUp = (e) => {
                const c = _avatarCrop;
                if (!c) return;
                c.dragging = false;
                c.pointerId = null;
                try { stage.releasePointerCapture(e.pointerId); } catch (err) { /* уже отпущен */ }
                stage.classList.remove("is-dragging");
            };

            const onWheel = (e) => {
                const c = _avatarCrop;
                if (!c) return;
                e.preventDefault();
                // Колесо меняет масштаб от центра круга: зум «в точку курсора»
                // выглядит богаче, но на тачпаде уводит картинку из-под руки.
                const step = e.deltaY < 0 ? 1.08 : 1 / 1.08;
                window.avatarCropSetZoom((c.scale * step) / c.base);
            };

            stage.addEventListener("pointerdown", onDown);
            stage.addEventListener("pointermove", onMove);
            stage.addEventListener("pointerup", onUp);
            stage.addEventListener("pointercancel", onUp);
            stage.addEventListener("wheel", onWheel, { passive: false });

            _avatarCrop.listeners = { onDown: onDown, onMove: onMove, onUp: onUp, onWheel: onWheel };

            _avatarCropApply();
            resolve({ width: nw, height: nh });
        };
        img.onerror = () => reject(new Error("Image failed to load."));
        img.src = dataUrl;
    });
};

// zoom — множитель к минимальному масштабу, 1 = картинка ровно закрывает круг.
window.avatarCropSetZoom = function avatarCropSetZoom(zoom) {
    const c = _avatarCrop;
    if (!c) return 1;

    const clamped = Math.min(4, Math.max(1, zoom));
    c.scale = c.base * clamped;
    _avatarCropApply();
    return clamped;
};

window.avatarCropExport = function avatarCropExport(outSize, quality) {
    const c = _avatarCrop;
    if (!c) throw new Error("Crop editor is not initialised.");

    const side = c.d / c.scale;                       // сторона выреза в пикселях исходника
    const sx = c.nw / 2 - c.tx / c.scale - side / 2;
    const sy = c.nh / 2 - c.ty / c.scale - side / 2;

    const out = Math.min(Math.round(side), outSize);  // не растягиваем мелкий исходник
    const canvas = document.createElement("canvas");
    canvas.width = out;
    canvas.height = out;

    const ctx = canvas.getContext("2d");
    ctx.imageSmoothingQuality = "high";
    ctx.drawImage(c.img, sx, sy, side, side, 0, 0, out, out);

    const tryExport = (type) => new Promise((resolve, reject) => {
        canvas.toBlob((blob) => {
            if (blob) resolve(blob);
            else reject(new Error("Canvas export to " + type + " failed."));
        }, type, quality);
    });

    return tryExport("image/webp")
        .catch(() => tryExport("image/jpeg"))
        .then((blob) => new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = () => resolve({
                dataUrl: reader.result,
                contentType: blob.type,
                byteLength: blob.size,
                width: out,
                height: out,
            });
            reader.onerror = () => reject(new Error("Reading cropped blob failed."));
            reader.readAsDataURL(blob);
        }));
};

window.avatarCropDispose = function avatarCropDispose() {
    const c = _avatarCrop;
    if (!c) return;

    const l = c.listeners || {};
    c.stage.removeEventListener("pointerdown", l.onDown);
    c.stage.removeEventListener("pointermove", l.onMove);
    c.stage.removeEventListener("pointerup", l.onUp);
    c.stage.removeEventListener("pointercancel", l.onUp);
    c.stage.removeEventListener("wheel", l.onWheel);

    if (c.img && c.img.parentNode) c.img.parentNode.removeChild(c.img);

    _avatarCrop = null;
};

// Исходник для редактора: только декодируем и, если картинка огромная,
// уменьшаем — резать будет уже пользователь. Верхняя граница нужна, чтобы
// снимок с телефона на 12 мегапикселей не жил в памяти WebView целиком.
function _avatarSourceFromImage(img, maxDim) {
    const nw = img.naturalWidth || img.width;
    const nh = img.naturalHeight || img.height;
    const factor = Math.min(1, maxDim / Math.max(nw, nh));

    if (factor >= 1) {
        return Promise.resolve({ dataUrl: img.src, width: nw, height: nh });
    }

    const canvas = document.createElement("canvas");
    canvas.width = Math.round(nw * factor);
    canvas.height = Math.round(nh * factor);

    const ctx = canvas.getContext("2d");
    ctx.imageSmoothingQuality = "high";
    ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

    return new Promise((resolve, reject) => {
        canvas.toBlob((blob) => {
            if (!blob) { reject(new Error("Canvas export failed.")); return; }
            const reader = new FileReader();
            reader.onload = () => resolve({
                dataUrl: reader.result,
                width: canvas.width,
                height: canvas.height,
            });
            reader.onerror = () => reject(new Error("Reading source blob failed."));
            reader.readAsDataURL(blob);
        }, "image/webp", 0.92);
    });
}

window.avatarSourceFromInputElement = async function avatarSourceFromInputElement(inputElement, maxDim) {
    if (!inputElement || !inputElement.files || inputElement.files.length === 0) {
        throw new Error("No file selected.");
    }
    const objectUrl = URL.createObjectURL(inputElement.files[0]);
    try {
        const img = await _loadImageFromObjectUrl(objectUrl, false);
        return await _avatarSourceFromImage(img, maxDim);
    } finally {
        URL.revokeObjectURL(objectUrl);
    }
};

window.avatarSourceFromDataUrl = async function avatarSourceFromDataUrl(dataUrl, maxDim) {
    const img = await _loadImageFromObjectUrl(dataUrl, false);
    return await _avatarSourceFromImage(img, maxDim);
};

window.avatarSourceFromUrl = async function avatarSourceFromUrl(url, maxDim) {
    const img = await _loadImageFromObjectUrl(url, true);
    return await _avatarSourceFromImage(img, maxDim);
};

// ─────────────────────── keyboard layout detection ───────────────────────
// The desktop host's IInputLanguageService never raises LanguageChange and
// its CurrentInputLanguage getter throws NotImplementedException, so the
// shell has no way of hearing about an Alt+Shift layout switch from the
// C# side. The WebView is Chromium though, and Chromium exposes the live
// OS layout through navigator.keyboard.getLayoutMap(). Polling that is the
// only route to the information that does not require changing the host.
//
// The 'layoutchange' event on navigator.keyboard exists in the spec but is
// not shipped in most Chromium builds, hence the poll rather than a
// listener.
let _layoutWatchTimer = null;
let _layoutWatchLast = null;
let _layoutWatchTick = null;
let _layoutWatchInterval = 800;

async function _probeLayoutSampleChar() {
    if (!navigator.keyboard || !navigator.keyboard.getLayoutMap) return null;
    try {
        const map = await navigator.keyboard.getLayoutMap();
        // KeyA is present in every layout worth distinguishing here and
        // its output identifies the script: "a" latin, "ф" cyrillic,
        // "α" greek, ...
        return map.get("KeyA") || null;
    } catch {
        return null;
    }
}

// Follows focus: a layout switch pressed into another window cannot change what
// this field will type, so there is nothing to poll for while the shell is behind.
function _layoutWatchResume() {
    if (_layoutWatchTimer !== null || _layoutWatchTick === null) return;

    _layoutWatchTick();
    _layoutWatchTimer = setInterval(_layoutWatchTick, _layoutWatchInterval);
}

function _layoutWatchSuspend() {
    if (_layoutWatchTimer === null) return;

    clearInterval(_layoutWatchTimer);
    _layoutWatchTimer = null;
}

window.setupInputLayoutWatch = function setupInputLayoutWatch(dotNetRef, callbackName, intervalMs) {
    window.teardownInputLayoutWatch();

    _layoutWatchInterval = intervalMs || 800;
    _layoutWatchTick = async () => {
        const sample = await _probeLayoutSampleChar();
        if (sample && sample !== _layoutWatchLast) {
            _layoutWatchLast = sample;
            try {
                await dotNetRef.invokeMethodAsync(callbackName, sample);
            } catch {
                // Component went away between the poll and the callback.
                window.teardownInputLayoutWatch();
            }
        }
    };

    if (window.GrafitActivity.isFocused) _layoutWatchResume();
};

window.teardownInputLayoutWatch = function teardownInputLayoutWatch() {
    _layoutWatchSuspend();

    _layoutWatchTick = null;
    _layoutWatchLast = null;
};

// Writing direction for the current language.
//
// Nothing the client ships today is right-to-left - the eleven translated cultures are
// all LTR - so this always resolves to "ltr" in practice. It exists because the document
// has to declare a direction for any RTL work to have something to switch on: without it
// even a fully mirrored stylesheet would never activate. See THIRD-PARTY-NOTICES.md's
// neighbour, the RTL section of deploy/README.md, for what mirroring the stylesheet would
// still involve.
window.setDocumentDirection = function setDocumentDirection(direction) {
    const value = direction === "rtl" ? "rtl" : "ltr";

    if (document.documentElement.getAttribute("dir") !== value) {
        document.documentElement.setAttribute("dir", value);
    }
};

// ───────────────────────────── accent palette ────────────────────────────
// Every colour the shell has an opinion about derives from one accent. Eight palettes
// are compiled into the stylesheet (src/scss/themes/client/_palette.scss) and chosen by
// the data-accent attribute on <html>; any other colour is derived here, at run time,
// with the same rules, and written onto <html> as the same custom properties. This is
// the single place either is written from.
//
// Where the value comes from, in order:
//   1. The skin's index.html: <html data-accent="blue"> - the skin's default.
//   2. A club's own stylesheet from the Manager (the StyleSheet option, served as
//      style.css): `:root { --gg-palette: green; }` or `:root { --gg-palette: #e11d48; }`.
//      The property is read back off the document once that stylesheet has loaded, so
//      a club changes its colour from the Manager without touching the skin. A Manager
//      setting for the colour, should Gizmo add one, only has to emit that line - or
//      call grafitTheme.set().
//   3. grafitTheme.set(nameOrColour) from anywhere at run time.
(function () {
    const ACCENTS = ["blue", "purple", "red", "orange", "amber", "green", "teal", "pink"];
    const DEFAULT = ACCENTS[0];
    const CUSTOM = "custom";
    const root = document.documentElement;

    // ── colour maths, matching the Sass functions in _palette.scss ──────────
    function clamp01(v) {
        return Math.min(1, Math.max(0, v));
    }

    // "#rgb", "#rrggbb", "rgb(r, g, b)" -> [r, g, b] in 0..255, or null.
    function parseColour(text) {
        const value = String(text || "").trim().replace(/^["']|["']$/g, "").toLowerCase();
        let m = value.match(/^#([0-9a-f]{3})$/);

        if (m) {
            return m[1].split("").map(function (c) { return parseInt(c + c, 16); });
        }

        m = value.match(/^#([0-9a-f]{6})$/);

        if (m) {
            return [0, 2, 4].map(function (i) { return parseInt(m[1].substr(i, 2), 16); });
        }

        m = value.match(/^rgba?\(\s*(\d{1,3})\s*[, ]\s*(\d{1,3})\s*[, ]\s*(\d{1,3})/);

        if (m) {
            return [m[1], m[2], m[3]].map(function (n) { return Math.min(255, parseInt(n, 10)); });
        }

        return null;
    }

    function rgbToHsl(rgb) {
        const r = rgb[0] / 255, g = rgb[1] / 255, b = rgb[2] / 255;
        const max = Math.max(r, g, b), min = Math.min(r, g, b);
        const l = (max + min) / 2;
        let h = 0, s = 0;

        if (max !== min) {
            const d = max - min;
            s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
            if (max === r) h = (g - b) / d + (g < b ? 6 : 0);
            else if (max === g) h = (b - r) / d + 2;
            else h = (r - g) / d + 4;
            h *= 60;
        }

        return [h, s, l];
    }

    function hslToRgb(h, s, l) {
        h = ((h % 360) + 360) % 360;
        s = clamp01(s);
        l = clamp01(l);
        const c = (1 - Math.abs(2 * l - 1)) * s;
        const x = c * (1 - Math.abs((h / 60) % 2 - 1));
        const m = l - c / 2;
        let r, g, b;

        if (h < 60) { r = c; g = x; b = 0; }
        else if (h < 120) { r = x; g = c; b = 0; }
        else if (h < 180) { r = 0; g = c; b = x; }
        else if (h < 240) { r = 0; g = x; b = c; }
        else if (h < 300) { r = x; g = 0; b = c; }
        else { r = c; g = 0; b = x; }

        return [(r + m) * 255, (g + m) * 255, (b + m) * 255];
    }

    function triplet(rgb) {
        return rgb.map(function (v) { return Math.round(v); }).join(", ");
    }

    function css(rgb) {
        return "rgb(" + triplet(rgb) + ")";
    }

    // color.adjust($c, $hue: deg)
    function rotate(rgb, deg) {
        const hsl = rgbToHsl(rgb);
        return hslToRgb(hsl[0] + deg, hsl[1], hsl[2]);
    }

    // color.scale($c, $lightness: -pct%)
    function darken(rgb, pct) {
        const hsl = rgbToHsl(rgb);
        return hslToRgb(hsl[0], hsl[1], hsl[2] * (1 - pct));
    }

    // color.mix(#fff, $c, pct%)
    function lighten(rgb, pct) {
        return rgb.map(function (v) { return 255 * pct + v * (1 - pct); });
    }

    // ── the rules (see _palette.scss: gg-is-red, gg-is-warm, gg-dark, gg-ink) ──
    function isRed(h) { return h >= 340 || h < 12; }
    function isWarm(h) { return h >= 15 && h <= 75; }

    function tokensFor(accent) {
        const h = rgbToHsl(accent)[0];
        const tint = isWarm(h) ? 0.35 : 1;
        const shift = isRed(h) ? 0 : -16;
        const dark = function (l, s) { return hslToRgb(h + shift, s * tint, l); };
        const ink = function (l, s) { return hslToRgb(h - 11, s * tint, l); };
        const inkMain = ink(0.95, 0.30);

        return {
            "--gg-accent": css(accent),
            "--gg-accent-rgb": triplet(accent),
            "--gg-accent-2-rgb": triplet(rotate(accent, isRed(h) ? -8 : -22)),
            "--gg-accent-3-rgb": triplet(rotate(accent, isRed(h) ? 16 : 28)),
            "--gg-accent-deep": css(darken(accent, 0.22)),
            "--gg-accent-deeper": css(darken(accent, 0.45)),
            "--gg-accent-light": css(lighten(accent, 0.18)),
            "--gg-accent-soft": css(lighten(accent, 0.45)),
            "--gg-accent-pale": css(lighten(accent, 0.62)),
            "--gg-on-accent": css(hslToRgb(h + 8, 0.59, 0.08)),
            "--gg-ink": css(inkMain),
            "--gg-ink-rgb": triplet(inkMain),
            "--gg-ink-2": css(ink(0.84, 0.22)),
            "--gg-ink-3": css(ink(0.72, 0.42)),
            "--gg-bg-0": css(dark(0.04, 0.50)),
            "--gg-bg-1": css(dark(0.06, 0.33)),
            "--gg-bg-2": css(dark(0.11, 0.41)),
            "--gg-bg-3": css(dark(0.09, 0.38)),
            "--gg-panel": css(dark(0.12, 0.38)),
            "--gg-tile": css(dark(0.09, 0.26)),
            "--gg-tile-2": css(dark(0.08, 0.30)),
            "--gg-tile-deep": css(dark(0.04, 0.45)),
            "--gg-glass-rgb": triplet(dark(0.17, 0.32)),
            "--gg-glass-2-rgb": triplet(dark(0.08, 0.33)),
            "--gg-scrim-rgb": triplet(dark(0.04, 0.40)),
            "--gg-lock-rgb": triplet(dark(0.13, 0.30))
        };
    }

    let customTokens = null;

    function clearCustom() {
        if (!customTokens) {
            return;
        }

        Object.keys(customTokens).forEach(function (name) {
            root.style.removeProperty(name);
        });
        customTokens = null;
    }

    function applyCustom(accent) {
        const tokens = tokensFor(accent);

        Object.keys(tokens).forEach(function (name) {
            root.style.setProperty(name, tokens[name]);
        });
        customTokens = tokens;
    }

    let current = null;

    const theme = {
        accents: ACCENTS.slice(),

        // The value in force: a palette name, or the colour a club named.
        get: function () {
            return current || root.getAttribute("data-accent") || DEFAULT;
        },

        // A palette name, or any colour ("#e11d48", "rgb(225, 29, 72)"). Returns false
        // for anything else, leaving the shell as it was.
        set: function (value) {
            const wanted = String(value || "").trim().replace(/^["']|["']$/g, "");
            const name = wanted.toLowerCase();

            if (ACCENTS.indexOf(name) >= 0) {
                if (current === name) {
                    return true;
                }

                clearCustom();
                root.setAttribute("data-accent", name);
                current = name;
                return true;
            }

            const colour = parseColour(wanted);

            if (!colour) {
                return false;
            }

            if (current === name) {
                return true;
            }

            // No compiled palette matches "custom", so the stylesheet falls back to the
            // default tokens and the inline ones written here win over them.
            root.setAttribute("data-accent", CUSTOM);
            applyCustom(colour);
            current = name;
            return true;
        },

        // Applies --gg-palette and --gg-motion from whatever stylesheets are loaded.
        // Called after the shell starts and again whenever a stylesheet finishes
        // loading, so the club's style.css wins over the skin's default no matter
        // which arrives first.
        sync: function () {
            const computed = getComputedStyle(root);
            const wanted = computed.getPropertyValue("--gg-palette");

            if (wanted) {
                theme.set(wanted);
            }

            // `:root { --gg-motion: on; }` turns the moving background on (_flow.scss).
            const motion = computed.getPropertyValue("--gg-motion");

            if (motion) {
                theme.motion(motion);
            }
        },

        // `:root { --gg-avatars: on; }` - or an address, when the club's picture
        // service does not sit on the server's own machine. Read once by the shell
        // after its first render; see AvatarService.Configure.
        avatars: function () {
            return String(getComputedStyle(root).getPropertyValue("--gg-avatars") || "")
                .trim()
                .replace(/["']/g, "");
        },

        // "on" or "off": the moving background behind the sign-in screen and the shell.
        // Off unless asked, so a club that never heard of it pays nothing for it.
        motion: function (value) {
            const wanted = String(value || "").trim().replace(/["']/g, "").toLowerCase();

            if (wanted === "on") {
                root.setAttribute("data-motion", "on");
                return true;
            }

            if (wanted === "off") {
                root.removeAttribute("data-motion");
                return true;
            }

            return false;
        }
    };

    document.addEventListener("load", function (event) {
        if (event.target && event.target.tagName === "LINK") {
            theme.sync();
        }
    }, true);

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", theme.sync);
    } else {
        theme.sync();
    }

    window.grafitTheme = theme;
})();

// ───────────────────────────── activity gate ─────────────────────────────
// The host never clears the WebView's IsVisible when its window goes behind
// another application, so document.hidden stays false and Chromium keeps
// painting this page at the monitor's refresh rate for nobody. Focus is the only
// usable signal, and it is also the right one: an application started outside
// Gizmo is invisible to the shell, so no cleverer condition would catch it.
//
// ShellActivityWatcher holds a veto on the .NET side and lifts sleeping while a
// deployment is running.
//
// Event driven with at most one pending timeout - a gate that polled to learn
// whether it was idle would be the cost it exists to remove.

// Long enough that a host-owned window taking focus for a moment does not read
// as leaving, and that entrance animations finish before anything is frozen.
const GRAFIT_IDLE_GRACE_MS = 2000;

let _grafitUnfocused = false;
// Starts closed and is only opened from .NET, so a document with no watcher -
// the notification window - never sleeps.
let _grafitIdleAllowed = false;
let _grafitIdle = false;

let _grafitIdleTimer = null;
let _grafitLastInput = 0;
let _grafitFocusSeen = false;
let _grafitPausedMedia = [];
let _grafitDotNetRef = null;
let _grafitDotNetCallback = null;
const _grafitListeners = [];

function _grafitApplyIdle() {
    const idle = _grafitUnfocused && _grafitIdleAllowed;
    if (idle === _grafitIdle) return;

    _grafitIdle = idle;

    // The attribute is what the stylesheet keys its animation-play-state off.
    // Paused rather than removed: an animation stopped this way resumes from the
    // same frame, so coming back to the shell is not a visible restart.
    if (idle) {
        document.documentElement.setAttribute("data-giz-idle", "1");
    } else {
        document.documentElement.removeAttribute("data-giz-idle");
    }

    // Video keeps decoding whatever CSS says, so it has to be told separately.
    // Only what we paused ourselves is resumed - a clip the shell had already
    // stopped stays stopped.
    if (idle) {
        _grafitPausedMedia = Array.prototype.filter.call(
            document.getElementsByTagName("video"),
            (video) => !video.paused && !video.ended
        );
        _grafitPausedMedia.forEach((video) => {
            try { video.pause(); } catch { /* element gone */ }
        });
    } else {
        const resume = _grafitPausedMedia;
        _grafitPausedMedia = [];
        resume.forEach((video) => {
            try { video.play(); } catch { /* element gone or not playable */ }
        });
    }

    _grafitListeners.forEach((listener) => {
        try { listener(!idle); } catch { /* one bad subscriber must not stop the rest */ }
    });
}

function _grafitSetUnfocused(value) {
    if (value === _grafitUnfocused) return;

    _grafitUnfocused = value;

    // The layout poll follows focus, not sleep: without the keyboard there is
    // nothing to poll for either way.
    if (value) _layoutWatchSuspend(); else _layoutWatchResume();

    // .NET receives the focus answer; it holds the other condition itself.
    _grafitPush(!value);

    _grafitApplyIdle();
}

// The .NET half is reached through an object reference handed over by
// ShellActivityWatcher, the same route every other callback in this shell uses.
// Nothing here depends on it being there: if the shell is not attached - not
// rendered yet, or torn down - the CSS half still does its job.
function _grafitPush(isFocused) {
    if (_grafitDotNetRef === null) return;

    try {
        _grafitDotNetRef.invokeMethodAsync(_grafitDotNetCallback, isFocused).catch(() => { });
    } catch {
        // Reference already disposed on the .NET side.
        _grafitDotNetRef = null;
    }
}

window.attachShellActivity = function attachShellActivity(dotNetRef, callbackName) {
    _grafitDotNetRef = dotNetRef;
    _grafitDotNetCallback = callbackName;

    // Answer straight away - the shell may already be in the background.
    _grafitPush(!_grafitUnfocused);
};

window.detachShellActivity = function detachShellActivity() {
    _grafitDotNetRef = null;

    // No .NET side means no permission to sleep - waking is the safe direction.
    window.setShellIdleAllowed(false);
};

/** Permission to sleep, from ShellActivityWatcher. Withdrawn during a deployment. */
window.setShellIdleAllowed = function setShellIdleAllowed(allowed) {
    allowed = !!allowed;
    if (allowed === _grafitIdleAllowed) return;

    _grafitIdleAllowed = allowed;

    _grafitApplyIdle();
};

function _grafitEvaluate() {
    if (_grafitIdleTimer !== null) {
        clearTimeout(_grafitIdleTimer);
        _grafitIdleTimer = null;
    }

    // Should the host ever start hiding the WebView properly, this is the cheap
    // path and Chromium has already stopped rendering by itself.
    if (document.visibilityState !== "visible") {
        _grafitSetUnfocused(true);
        return;
    }

    if (document.hasFocus()) {
        _grafitFocusSeen = true;
        _grafitSetUnfocused(false);
        return;
    }

    // Only trust "unfocused" once focus has been seen at least once. In a host
    // where the WebView never receives focus the answer means nothing, and leaving
    // the shell running is the only honest conclusion.
    if (!_grafitFocusSeen) {
        _grafitSetUnfocused(false);
        return;
    }

    const waited = Date.now() - _grafitLastInput;
    if (waited >= GRAFIT_IDLE_GRACE_MS) {
        _grafitSetUnfocused(true);
        return;
    }

    _grafitIdleTimer = setTimeout(_grafitEvaluate, GRAFIT_IDLE_GRACE_MS - waited);
}

// Input is the fallback signal. Mouse and keys can only reach this page while it
// is the window in front, so if focus reporting is ever unreliable the gate still
// opens the moment the customer touches anything.
function _grafitInput(e) {
    _grafitLastInput = Date.now();

    // A click or a keystroke landing here is proof the window can hold focus,
    // whatever hasFocus() says at this instant. Pointer moves are not: they arrive
    // over an unfocused window too.
    if (e && (e.type === "pointerdown" || e.type === "keydown")) _grafitFocusSeen = true;

    if (_grafitUnfocused) _grafitSetUnfocused(false);
}

function _grafitFocused() {
    _grafitFocusSeen = true;
    _grafitEvaluate();
}

window.GrafitActivity = {
    get isActive() { return !_grafitIdle; },
    get isFocused() { return !_grafitUnfocused; },

    subscribe(listener) {
        if (typeof listener === "function") _grafitListeners.push(listener);
    },

    unsubscribe(listener) {
        const index = _grafitListeners.indexOf(listener);
        if (index > -1) _grafitListeners.splice(index, 1);
    },
};

window.addEventListener("focus", _grafitFocused);
window.addEventListener("blur", _grafitEvaluate);
document.addEventListener("visibilitychange", _grafitEvaluate);

["pointerdown", "pointermove", "keydown", "wheel"].forEach((name) => {
    window.addEventListener(name, _grafitInput, { passive: true, capture: true });
});

_grafitEvaluate();
