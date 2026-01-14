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

window.registerAppsSticky = function registerAppsSticky() {
  var container = document.querySelector(".giz-apps__body__content");
  if (!container) return;

  container.addEventListener("scroll", appsSticky);
};

window.unregisterAppsSticky = function unregisterAppsSticky() {
  var container = document.querySelector(".giz-apps__body__content");
  if (!container) return;

  container.removeEventListener("scroll", appsSticky);
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
  container.addEventListener("scroll", autoHideAds);
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
