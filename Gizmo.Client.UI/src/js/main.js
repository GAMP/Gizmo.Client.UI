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