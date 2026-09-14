using System;
using System.Collections.Generic;
using System.Globalization;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Localization
{
    /// <summary>
    /// Every interface string this shell adds or rewords, in one place.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The localized strings live in <c>Gizmo.Client.UI.Resources.dll</c>, which belongs to
    /// the client on each machine rather than to the skin, so editing the .resx never
    /// reaches a club through a skin swap. Anything this shell needs to say therefore has
    /// to travel inside <c>Gizmo.Client.UI.dll</c>, which is what this table is.
    /// </para>
    /// <para>
    /// Two kinds of key, and the difference matters for clubs that do not run in Russian:
    /// </para>
    /// <list type="bullet">
    /// <item><description>
    /// A <c>GIZ_</c> key is the vendor's, already translated into every culture the client
    /// ships. It appears here only when this shell wants different wording, and only for
    /// the cultures that wording exists in - every other culture falls through to the
    /// vendor's own translation.
    /// </description></item>
    /// <item><description>
    /// A <c>SHELL_</c> key is ours outright, with nothing in the resource manager behind
    /// it. Those carry Russian and English here and the client's other eight cultures in
    /// <c>ShellStringOverrides.Translations.cs</c>; anything else falls back to English
    /// rather than showing a raw key.
    /// </description></item>
    /// </list>
    /// <para>
    /// Prefer a vendor key whenever one fits: it costs nothing and arrives translated.
    /// </para>
    /// </remarks>
    public static partial class ShellStringOverrides
    {
        private const string SHELL_PREFIX = "SHELL_";

        /// <summary>
        /// Folds the other cultures into the table. Field initialisers of both files have
        /// run by the time a static constructor starts, whichever file the compiler took
        /// first. Russian and English here win over anything the translations file says.
        /// </summary>
        static ShellStringOverrides()
        {
            foreach (var (key, byCulture) in _translations)
            {
                if (!_overrides.TryGetValue(key, out var existing))
                {
                    _overrides[key] = new Dictionary<string, string>(byCulture);
                    continue;
                }

                foreach (var (culture, text) in byCulture)
                    existing.TryAdd(culture, text);
            }
        }

        #region KEYS

        // ── Vendor keys reused as-is or reworded below ──────────────────────────────
        public const string GEN_CLOSE = "GIZ_GEN_CLOSE";
        public const string GEN_CANCEL = "GIZ_GEN_CANCEL";
        public const string GEN_BACK = "GIZ_GEN_BACK";
        public const string GEN_ALL = "GIZ_GEN_ALL";
        public const string GEN_ADD_TO_CART = "GIZ_GEN_ADD_TO_CART";
        public const string GEN_BALANCE = "GIZ_GEN_BALANCE";
        public const string GEN_ERROR = "GIZ_GEN_AN_ERROR_HAS_OCCURRED";
        public const string GEN_LAUNCH = "GIZ_GEN_LAUNCH";
        public const string GEN_DETAILS = "GIZ_GEN_DETAILS";
        public const string SHOP_TOTAL = "GIZ_SHOP_TOTAL";
        public const string SHOP_PRICE = "GIZ_SHOP_PRICE";
        public const string SHOP_PLACE_ORDER = "GIZ_SHOP_PLACE_ORDER";
        public const string SHOP_CHECKOUT_TITLE = "GIZ_SHOP_CHECKOUT_TITLE";
        public const string PRICE_OR = "GIZ_PRODUCT_PRICE_PURCHASE_OPTION_OR";
        public const string PRICE_AND = "GIZ_PRODUCT_PRICE_PURCHASE_OPTION_AND";
        public const string PRICE_FREE = "GIZ_PRODUCT_PRICE_FREE";
        public const string SHOP_PAYMENT_METHOD = "GIZ_SHOP_PAYMENT_METHOD";
        public const string POPULAR_APPS = "GIZ_GEN_POPULAR_APPS";
        public const string ALL_APPS = "GIZ_APP_FILTERS_ALL_APPS";
        public const string PURCHASES = "GIZ_USER_PURCHASES";
        public const string NOTIFICATIONS_TITLE = "GIZ_NOTIFICATIONS_TITLE";
        public const string NOTIFICATIONS_EMPTY = "GIZ_NOTIFICATIONS_NO_NEW_NOTIFICATIONS";
        public const string NOTIFICATIONS_CLEAR = "GIZ_NOTIFICATIONS_MARK_ALL_AS_READ";
        public const string ACTIVE_APPS_TITLE = "GIZ_ACTIVE_APPS_TITLE";
        public const string SEARCH_PLACEHOLDER = "GIZ_GLOBAL_SEARCH_PLACEHOLDER";
        public const string SEARCH_APPS = "GIZ_GLOBAL_SEARCH_GAMES_AND_APPS";
        public const string SEARCH_SHOP = "GIZ_GLOBAL_SEARCH_SHOP";
        public const string SEARCH_SHOW_ALL = "GIZ_GLOBAL_SEARCH_SHOW_ALL";
        public const string DEPOSIT_TITLE = "GIZ_ONLINE_DEPOSIT_TITLE";
        public const string DEPOSIT_QUICK_SELECT = "GIZ_ONLINE_DEPOSIT_QUICK_SELECT";
        public const string DEPOSIT_OR_ENTER_AMOUNT = "GIZ_ONLINE_DEPOSIT_OR_ENTER_AMOUNT";
        public const string DEPOSIT_WILL_BE_CREDITED = "GIZ_ONLINE_DEPOSIT_WILL_BE_CREDITED";
        public const string DEPOSIT_TO_BE_PAID = "GIZ_ONLINE_DEPOSIT_TO_BE_PAID";
        public const string DEPOSIT_QR_CODE = "GIZ_ONLINE_DEPOSIT_QR_CODE";
        public const string DEPOSIT_PAY_FROM_PC = "GIZ_ONLINE_DEPOSIT_PAY_FROM_PC";
        public const string DEPOSIT_CLEAR = "GIZ_ONLINE_DEPOSIT_CLEAR";
        public const string DEPOSIT_OR = "GIZ_ONLINE_DEPOSIT_OR";
        public const string PAY = "GIZ_CONFIRM_RESERVATION_DIALOG_PAY_BUTTON";
        public const string GO_TO_PAYMENT = "GIZ_CONFIRM_RESERVATION_NOTIFICATION_PAYMENT_GO_TO_PAYMENT_BUTTON";
        public const string LOGOUT = "GIZ_GRACE_PERIOD_LOGOUT";
        public const string TIME_PRODUCT = "GIZ_USER_TIME_PRODUCTS_TIME_PRODUCT";
        public const string TIME_TYPE_PACKAGE = "GIZ_USER_TIME_PRODUCTS_TITLE_TIME_OFFER";
        public const string TIME_TYPE_FIXED = "GIZ_USER_TIME_PRODUCTS_TITLE_FIXED_TIME";
        public const string TIME_TYPE_RATE = "GIZ_USER_TIME_PRODUCTS_TITLE_RATE";
        public const string TIME_TYPE_NONE = "GIZ_USAGE_TYPE_NONE";
        public const string TIME_IN_CREDIT = "GIZ_USER_TIME_PRODUCTS_TITLE_USING_CREDIT";
        public const string TIME_REMAINING = "GIZ_USER_TIME_PRODUCTS_REMAINING_TIME";
        public const string TIME_USABLE_HERE = "GIZ_USER_TIME_PRODUCTS_USABLE_TIME";
        public const string TIME_BOUGHT = "GIZ_USER_TIME_PRODUCTS_PURCHASE_DATE";
        public const string TIME_EXPIRES = "GIZ_USER_TIME_PRODUCTS_EXPIRES";
        public const string TIME_EXPIRES_FROM_USE = "GIZ_USER_TIME_PRODUCTS_PROPERTIES_EXPIRES_FROM_USE";
        public const string TIME_EXPIRES_FROM_PURCHASE = "GIZ_USER_TIME_PRODUCTS_PROPERTIES_EXPIRES_FROM_PURCHASE";
        public const string TIME_EXPIRES_AT_LOGOUT = "GIZ_USER_TIME_PRODUCTS_PROPERTIES_EXPIRES_LOGOUT";
        public const string TIME_ACTIVATED = "GIZ_USER_TIME_PRODUCTS_PROPERTIES_EXPIRES_ACTIVATED";
        public const string TIME_HOSTS = "GIZ_USER_TIME_PRODUCTS_HOSTS";
        public const string TIME_PRODUCTS_TITLE = "GIZ_USER_TIME_PRODUCTS_TITLE";
        public const string EXPIRE_DAYS_ABBR = "GIZ_PRODUCT_TIME_EXPIRATION_DAYS_ABBREVIATED";
        public const string EXPIRE_HOURS_ABBR = "GIZ_PRODUCT_TIME_EXPIRATION_HOURS_ABBREVIATED";
        public const string EXPIRE_MINUTES_ABBR = "GIZ_PRODUCT_TIME_EXPIRATION_MINUTES_ABBREVIATED";
        public const string USER_DETAILS = "GIZ_USER_DETAILS";
        public const string USER_AVAILABLE_TIME = "GIZ_USER_AVAILABLE_TIME";
        public const string USER_CONTACT_INFO = "GIZ_USER_CONTACT_INFO";
        public const string USER_SECURITY = "GIZ_USER_SECURITY";
        public const string USER_CHANGE_PASSWORD = "GIZ_USER_CHANGE_PASSWORD";
        public const string GEN_EMAIL_ADDRESS = "GIZ_GEN_EMAIL_ADDRESS";
        public const string GEN_PHONE = "GIZ_GEN_PHONE";
        public const string GEN_PASSWORD = "GIZ_GEN_PASSWORD";
        public const string GEN_GUEST = "GIZ_GEN_GUEST";
        public const string GEN_POINTS = "GIZ_GEN_POINTS";
        public const string GEN_TIME = "GIZ_GEN_TIME";
        public const string GEN_PIECES_ABBR = "GIZ_GEN_PIECES_ABBREVIATED";
        public const string CREDIT = "GIZ_USER_PROFILE_HEADER_CREDIT";
        public const string CREDIT_TIME_NOTE = "GIZ_USER_PROFILE_CREDIT_TOOLTIP_TIME_CREDIT_DESCRIPTION";
        public const string CREDIT_SALES_NOTE = "GIZ_USER_PROFILE_CREDIT_TOOLTIP_SALES_CREDIT_DESCRIPTION";
        public const string CREDIT_UNLIMITED = "GIZ_USER_PROFILE_CREDIT_TOOLTIP_UNLIMITED_CREDIT_DESCRIPTION";
        public const string PURCHASES_HISTORY = "GIZ_USER_PURCHASES_HISTORY";
        public const string ORDER_STATUS_VOIDED = "GIZ_USER_PURCHASES_ORDER_STATUS_VOIDED";
        public const string ORDER_STATUS_ON_HOLD = "GIZ_USER_PURCHASES_ORDER_STATUS_ON_HOLD";
        public const string ORDER_STATUS_COMPLETED = "GIZ_USER_PURCHASES_ORDER_STATUS_COMPLETED";
        public const string ORDER_STATUS_CANCELED = "GIZ_USER_PURCHASES_ORDER_STATUS_CANCELED";
        public const string ORDER_STATUS_ACCEPTED = "GIZ_USER_PURCHASES_ORDER_STATUS_ACCEPTED";
        public const string INVOICE_UNPAID = "GIZ_USER_PURCHASES_ORDER_INVOICE_STATUS_UNPAID";
        public const string INVOICE_PARTIALLY_PAID = "GIZ_USER_PURCHASES_ORDER_INVOICE_STATUS_PARTIALLY_PAID";
        public const string INVOICE_PAID = "GIZ_USER_PURCHASES_ORDER_INVOICE_STATUS_PAID";
        public const string ORDER_EARNED_POINTS = "GIZ_USER_PURCHASES_EARNED_POINTS";
        public const string ORDER_NOTE = "GIZ_USER_PURCHASES_ORDER_NOTE";
        public const string ORDER_PAYMENT_METHOD = "GIZ_USER_PURCHASES_PAYMENT_METHOD";
        public const string SHOP_RELATED = "GIZ_SHOP_RELATED_PRODUCTS";
        public const string PRODUCT_BUY_WINDOW = "GIZ_PRODUCT_TIME_EXPIRATION_BUYING_TIME";
        public const string PRODUCT_USE_WINDOW = "GIZ_PRODUCT_TIME_EXPIRATION_ACTION_TIME";
        public const string PRODUCT_EXPIRY = "GIZ_PRODUCT_TIME_EXPIRES";
        public const string PRODUCT_MORE = "GIZ_PRODUCT_DETAILS_MORE";
        public const string PRODUCT_LESS = "GIZ_PRODUCT_DETAILS_LESS";
        public const string HOUR_ABBREVIATED = "GIZ_PRODUCT_TIME_EXPIRATION_HOUR_ABBREVIATED";
        public const string MINUTE_ABBREVIATED = "GIZ_PRODUCT_TIME_EXPIRATION_MINUTE_ABBREVIATED";

        // ── Keys this shell owns ────────────────────────────────────────────────────
        public const string HERO_EMPTY = SHELL_PREFIX + "HERO_EMPTY";
        public const string HERO_PROMOS = SHELL_PREFIX + "HERO_PROMOS";
        public const string HERO_POPULAR = SHELL_PREFIX + "HERO_POPULAR";
        public const string HOME_GAMES_SECTION = SHELL_PREFIX + "HOME_GAMES_SECTION";
        public const string HOME_TIME_PACKAGES = SHELL_PREFIX + "HOME_TIME_PACKAGES";
        public const string HOME_NO_PACKAGES = SHELL_PREFIX + "HOME_NO_PACKAGES";
        public const string HOME_BAR = SHELL_PREFIX + "HOME_BAR";
        public const string HOME_WHOLE_SHOP = SHELL_PREFIX + "HOME_WHOLE_SHOP";
        public const string GEN_EMPTY = SHELL_PREFIX + "GEN_EMPTY";
        public const string GEN_BUY = SHELL_PREFIX + "GEN_BUY";
        public const string GEN_WORKING = SHELL_PREFIX + "GEN_WORKING";
        public const string GEN_DONE = SHELL_PREFIX + "GEN_DONE";
        public const string GEN_LATER = SHELL_PREFIX + "GEN_LATER";
        public const string GEN_HIDE = SHELL_PREFIX + "GEN_HIDE";
        public const string GEN_TO_SHOP = SHELL_PREFIX + "GEN_TO_SHOP";
        public const string DEPOSIT_SUCCESS_TITLE = SHELL_PREFIX + "DEPOSIT_SUCCESS_TITLE";
        public const string DEPOSIT_SUCCESS_TEXT = SHELL_PREFIX + "DEPOSIT_SUCCESS_TEXT";
        public const string DEPOSIT_SUCCESS_HINT = SHELL_PREFIX + "DEPOSIT_SUCCESS_HINT";
        public const string DEPOSIT_PAGE_TITLE = SHELL_PREFIX + "DEPOSIT_PAGE_TITLE";
        public const string BUY_TITLE = SHELL_PREFIX + "BUY_TITLE";
        public const string BUY_TITLE_TOPUP = SHELL_PREFIX + "BUY_TITLE_TOPUP";
        public const string BUY_DONE_TITLE = SHELL_PREFIX + "BUY_DONE_TITLE";
        public const string BUY_FALLBACK_NAME = SHELL_PREFIX + "BUY_FALLBACK_NAME";
        public const string BUY_SHORTFALL = SHELL_PREFIX + "BUY_SHORTFALL";
        public const string BUY_DISCOUNT = SHELL_PREFIX + "BUY_DISCOUNT";
        public const string BUY_FREE = SHELL_PREFIX + "BUY_FREE";
        public const string BUY_TOP_UP_BY = SHELL_PREFIX + "BUY_TOP_UP_BY";
        public const string BUY_TOP_UP_AT_COUNTER = SHELL_PREFIX + "BUY_TOP_UP_AT_COUNTER";
        public const string BUY_TOPUP_SUB = SHELL_PREFIX + "BUY_TOPUP_SUB";
        public const string BUY_PAY_WITH = SHELL_PREFIX + "BUY_PAY_WITH";
        public const string BUY_WAY_BALANCE = SHELL_PREFIX + "BUY_WAY_BALANCE";
        public const string BUY_WAY_POINTS = SHELL_PREFIX + "BUY_WAY_POINTS";
        public const string BUY_WAY_COUNTER_HINT = SHELL_PREFIX + "BUY_WAY_COUNTER_HINT";
        public const string BUY_COUNTER_NOTE = SHELL_PREFIX + "BUY_COUNTER_NOTE";
        public const string BUY_ON_ACCOUNT = SHELL_PREFIX + "BUY_ON_ACCOUNT";
        public const string BUY_YOU_HAVE = SHELL_PREFIX + "BUY_YOU_HAVE";
        public const string BUY_OTHER_ITEMS = SHELL_PREFIX + "BUY_OTHER_ITEMS";
        public const string BUY_FEES = SHELL_PREFIX + "BUY_FEES";
        public const string BUY_LEFT_AFTER = SHELL_PREFIX + "BUY_LEFT_AFTER";
        public const string BUY_POINTS_LEFT_AFTER = SHELL_PREFIX + "BUY_POINTS_LEFT_AFTER";
        public const string BUY_BUY_FOR = SHELL_PREFIX + "BUY_BUY_FOR";
        public const string BUY_GET_FREE = SHELL_PREFIX + "BUY_GET_FREE";
        public const string BUY_ORDERED_TITLE = SHELL_PREFIX + "BUY_ORDERED_TITLE";
        public const string BUY_ORDERED_HINT = SHELL_PREFIX + "BUY_ORDERED_HINT";
        public const string BUY_NO_WAYS = SHELL_PREFIX + "BUY_NO_WAYS";
        public const string BUY_FAILED_TITLE = SHELL_PREFIX + "BUY_FAILED_TITLE";
        public const string BUY_AT_COUNTER = SHELL_PREFIX + "BUY_AT_COUNTER";
        public const string BUY_AT_COUNTER_NOTE = SHELL_PREFIX + "BUY_AT_COUNTER_NOTE";
        public const string BUY_ORDER_FOR = SHELL_PREFIX + "BUY_ORDER_FOR";
        public const string BUY_ORDER_PAID_TITLE = SHELL_PREFIX + "BUY_ORDER_PAID_TITLE";
        public const string BUY_ITEMS_COUNT = SHELL_PREFIX + "BUY_ITEMS_COUNT";
        public const string BUY_POINTS_COUNT = SHELL_PREFIX + "BUY_POINTS_COUNT";
        public const string LOGIN_PIN_PLACEHOLDER = SHELL_PREFIX + "LOGIN_PIN_PLACEHOLDER";
        public const string RESV_PC_RESERVED = SHELL_PREFIX + "RESV_PC_RESERVED";
        public const string RESV_DEVICE_RESERVED = SHELL_PREFIX + "RESV_DEVICE_RESERVED";
        public const string RESV_FROM = SHELL_PREFIX + "RESV_FROM";
        public const string RESV_IN = SHELL_PREFIX + "RESV_IN";
        public const string RESV_NOTIF_TITLE_CODE = SHELL_PREFIX + "RESV_NOTIF_TITLE_CODE";
        public const string RESV_NOTIF_TITLE_PAY = SHELL_PREFIX + "RESV_NOTIF_TITLE_PAY";
        public const string RESV_NOTIF_MSG_CODE = SHELL_PREFIX + "RESV_NOTIF_MSG_CODE";
        public const string RESV_NOTIF_MSG_PAY = SHELL_PREFIX + "RESV_NOTIF_MSG_PAY";
        public const string RESV_ENTER_CODE = SHELL_PREFIX + "RESV_ENTER_CODE";
        public const string GRACE_RESERVED_AT = SHELL_PREFIX + "GRACE_RESERVED_AT";
        public const string GRACE_RESERVED_HINT = SHELL_PREFIX + "GRACE_RESERVED_HINT";
        public const string GRACE_CONFIRMED = SHELL_PREFIX + "GRACE_CONFIRMED";
        public const string GRACE_CONFIRMED_UNPAID = SHELL_PREFIX + "GRACE_CONFIRMED_UNPAID";
        public const string GRACE_PIN_LABEL = SHELL_PREFIX + "GRACE_PIN_LABEL";
        public const string GRACE_TIME_LEFT = SHELL_PREFIX + "GRACE_TIME_LEFT";
        public const string GRACE_TIME_UP = SHELL_PREFIX + "GRACE_TIME_UP";
        public const string GRACE_TIME_UP_MESSAGE = SHELL_PREFIX + "GRACE_TIME_UP_MESSAGE";
        public const string GRACE_TOP_UP_ACCOUNT = SHELL_PREFIX + "GRACE_TOP_UP_ACCOUNT";
        public const string GEN_CONFIRM = SHELL_PREFIX + "GEN_CONFIRM";
        public const string GRACE_PIN_WRONG = SHELL_PREFIX + "GRACE_PIN_WRONG";
        public const string GRACE_PIN_FAILED = SHELL_PREFIX + "GRACE_PIN_FAILED";
        public const string DEPLOY_TITLE = SHELL_PREFIX + "DEPLOY_TITLE";
        public const string DEPLOY_TITLE_PREPARING = SHELL_PREFIX + "DEPLOY_TITLE_PREPARING";
        public const string DEPLOY_COPYING = SHELL_PREFIX + "DEPLOY_COPYING";
        public const string DEPLOY_LAUNCHING = SHELL_PREFIX + "DEPLOY_LAUNCHING";
        public const string DEPLOY_READY_COUNT = SHELL_PREFIX + "DEPLOY_READY_COUNT";
        public const string DEPLOY_RUNNING_COUNT = SHELL_PREFIX + "DEPLOY_RUNNING_COUNT";
        public const string DEPLOY_OPEN_APPS = SHELL_PREFIX + "DEPLOY_OPEN_APPS";
        public const string ACTIVE_APPS_HINT = SHELL_PREFIX + "ACTIVE_APPS_HINT";
        public const string ACTIVE_APPS_EMPTY_HINT = SHELL_PREFIX + "ACTIVE_APPS_EMPTY_HINT";
        public const string NOTIFICATIONS_HINT = SHELL_PREFIX + "NOTIFICATIONS_HINT";
        public const string NOTIFICATIONS_EMPTY_TITLE = SHELL_PREFIX + "NOTIFICATIONS_EMPTY_TITLE";
        public const string TIME_TOOLTIP_TITLE = SHELL_PREFIX + "TIME_TOOLTIP_TITLE";
        public const string TIME_TOOLTIP_ALL_PACKAGES = SHELL_PREFIX + "TIME_TOOLTIP_ALL_PACKAGES";
        public const string TIME_TOOLTIP_LEFT = SHELL_PREFIX + "TIME_TOOLTIP_LEFT";
        public const string TIME_TOOLTIP_SPENDABLE = SHELL_PREFIX + "TIME_TOOLTIP_SPENDABLE";
        public const string TIME_TOOLTIP_UNLIMITED = SHELL_PREFIX + "TIME_TOOLTIP_UNLIMITED";
        public const string TIME_TOOLTIP_MONEY_LOW = SHELL_PREFIX + "TIME_TOOLTIP_MONEY_LOW";
        public const string TIME_TOOLTIP_ENDING = SHELL_PREFIX + "TIME_TOOLTIP_ENDING";
        public const string TIME_TOOLTIP_CTA_BOTH = SHELL_PREFIX + "TIME_TOOLTIP_CTA_BOTH";
        public const string TIME_TOOLTIP_CTA_PACKAGE = SHELL_PREFIX + "TIME_TOOLTIP_CTA_PACKAGE";
        public const string TIME_TOOLTIP_CTA_TOPUP = SHELL_PREFIX + "TIME_TOOLTIP_CTA_TOPUP";
        public const string TIME_TOOLTIP_CREDIT = SHELL_PREFIX + "TIME_TOOLTIP_CREDIT";
        public const string DURATION_HOURS_MINUTES = SHELL_PREFIX + "DURATION_HOURS_MINUTES";
        public const string DURATION_HOURS = SHELL_PREFIX + "DURATION_HOURS";
        public const string DURATION_MINUTES = SHELL_PREFIX + "DURATION_MINUTES";
        public const string TIME_TOOLTIP_EMPTY = SHELL_PREFIX + "TIME_TOOLTIP_EMPTY";
        public const string ACCOUNT_TAB_PROFILE = SHELL_PREFIX + "ACCOUNT_TAB_PROFILE";
        public const string ACCOUNT_TAB_TIME = SHELL_PREFIX + "ACCOUNT_TAB_TIME";
        public const string ACCOUNT_MEMBER_SINCE = SHELL_PREFIX + "ACCOUNT_MEMBER_SINCE";
        public const string ACCOUNT_NOT_SET = SHELL_PREFIX + "ACCOUNT_NOT_SET";
        public const string ACCOUNT_PASSWORD_NOTE = SHELL_PREFIX + "ACCOUNT_PASSWORD_NOTE";
        public const string ACCOUNT_CONTACTS_NOTE = SHELL_PREFIX + "ACCOUNT_CONTACTS_NOTE";
        public const string ACCOUNT_NO_TIME = SHELL_PREFIX + "ACCOUNT_NO_TIME";
        public const string ACCOUNT_NO_TIME_HINT = SHELL_PREFIX + "ACCOUNT_NO_TIME_HINT";
        public const string ACCOUNT_NO_PURCHASES = SHELL_PREFIX + "ACCOUNT_NO_PURCHASES";
        public const string ACCOUNT_NO_PURCHASES_HINT = SHELL_PREFIX + "ACCOUNT_NO_PURCHASES_HINT";
        public const string ACCOUNT_TIME_HINT = SHELL_PREFIX + "ACCOUNT_TIME_HINT";
        public const string ACCOUNT_ACTIVE_NOW = SHELL_PREFIX + "ACCOUNT_ACTIVE_NOW";
        public const string ACCOUNT_QUEUE_POSITION = SHELL_PREFIX + "ACCOUNT_QUEUE_POSITION";
        public const string ACCOUNT_NOT_HERE = SHELL_PREFIX + "ACCOUNT_NOT_HERE";
        public const string ACCOUNT_ORDER_NUMBER = SHELL_PREFIX + "ACCOUNT_ORDER_NUMBER";
        public const string ACCOUNT_NEWER = SHELL_PREFIX + "ACCOUNT_NEWER";
        public const string ACCOUNT_OLDER = SHELL_PREFIX + "ACCOUNT_OLDER";
        public const string ACCOUNT_EXPIRES_NEVER = SHELL_PREFIX + "ACCOUNT_EXPIRES_NEVER";
        public const string PD_INCLUDES = SHELL_PREFIX + "PD_INCLUDES";
        public const string PD_WHERE = SHELL_PREFIX + "PD_WHERE";
        public const string PD_ABOUT = SHELL_PREFIX + "PD_ABOUT";
        public const string PD_NO_DESCRIPTION = SHELL_PREFIX + "PD_NO_DESCRIPTION";
        public const string PD_UNAVAILABLE = SHELL_PREFIX + "PD_UNAVAILABLE";
        public const string PD_EXPIRES_AT_DAYTIME = SHELL_PREFIX + "PD_EXPIRES_AT_DAYTIME";
        public const string PD_EXPIRES_AFTER = SHELL_PREFIX + "PD_EXPIRES_AFTER";
        public const string PD_EXPIRES_AT_LOGOUT = SHELL_PREFIX + "PD_EXPIRES_AT_LOGOUT";
        public const string PD_QUANTITY = SHELL_PREFIX + "PD_QUANTITY";

        // ── Loyalty: ladder level, achievements, challenges (Gizmo 3.0.95+) ──────────
        public const string LOYALTY_TAB = SHELL_PREFIX + "LOYALTY_TAB";
        public const string LOYALTY_HOME_CAP = SHELL_PREFIX + "LOYALTY_HOME_CAP";
        public const string LOYALTY_MORE = SHELL_PREFIX + "LOYALTY_MORE";
        public const string LOYALTY_LEVEL_CAP = SHELL_PREFIX + "LOYALTY_LEVEL_CAP";
        public const string LOYALTY_STATE_EARNING = SHELL_PREFIX + "LOYALTY_STATE_EARNING";
        public const string LOYALTY_STATE_SECURED = SHELL_PREFIX + "LOYALTY_STATE_SECURED";
        public const string LOYALTY_STATE_AWAITING = SHELL_PREFIX + "LOYALTY_STATE_AWAITING";
        public const string LOYALTY_RETAIN_SHORT = SHELL_PREFIX + "LOYALTY_RETAIN_SHORT";
        public const string LOYALTY_RETAIN_LINE = SHELL_PREFIX + "LOYALTY_RETAIN_LINE";
        public const string LOYALTY_NEXT_SHORT = SHELL_PREFIX + "LOYALTY_NEXT_SHORT";
        public const string LOYALTY_NEXT_LINE = SHELL_PREFIX + "LOYALTY_NEXT_LINE";
        public const string LOYALTY_SECURED_SHORT = SHELL_PREFIX + "LOYALTY_SECURED_SHORT";
        public const string LOYALTY_SECURED_LINE = SHELL_PREFIX + "LOYALTY_SECURED_LINE";
        public const string LOYALTY_TOP_LINE = SHELL_PREFIX + "LOYALTY_TOP_LINE";
        public const string LOYALTY_AWAITING_LINE = SHELL_PREFIX + "LOYALTY_AWAITING_LINE";
        public const string LOYALTY_REQUIREMENTS_LINE = SHELL_PREFIX + "LOYALTY_REQUIREMENTS_LINE";
        public const string LOYALTY_POINTS_PERIOD = SHELL_PREFIX + "LOYALTY_POINTS_PERIOD";
        public const string LOYALTY_PERIOD_DAY = SHELL_PREFIX + "LOYALTY_PERIOD_DAY";
        public const string LOYALTY_PERIOD_WEEK = SHELL_PREFIX + "LOYALTY_PERIOD_WEEK";
        public const string LOYALTY_PERIOD_MONTH = SHELL_PREFIX + "LOYALTY_PERIOD_MONTH";
        public const string LOYALTY_PERIOD_QUARTER = SHELL_PREFIX + "LOYALTY_PERIOD_QUARTER";
        public const string LOYALTY_PERIOD_YEAR = SHELL_PREFIX + "LOYALTY_PERIOD_YEAR";
        public const string LOYALTY_SETTLE = SHELL_PREFIX + "LOYALTY_SETTLE";
        public const string LOYALTY_START = SHELL_PREFIX + "LOYALTY_START";
        public const string LOYALTY_KEEP_AT = SHELL_PREFIX + "LOYALTY_KEEP_AT";
        public const string LOYALTY_BONUSES = SHELL_PREFIX + "LOYALTY_BONUSES";
        public const string LOYALTY_PERK_DISCOUNT = SHELL_PREFIX + "LOYALTY_PERK_DISCOUNT";
        public const string LOYALTY_PERK_BONUS = SHELL_PREFIX + "LOYALTY_PERK_BONUS";
        public const string LOYALTY_PERK_QUEUE = SHELL_PREFIX + "LOYALTY_PERK_QUEUE";
        public const string LOYALTY_CHALLENGES = SHELL_PREFIX + "LOYALTY_CHALLENGES";
        public const string LOYALTY_ACHIEVEMENTS = SHELL_PREFIX + "LOYALTY_ACHIEVEMENTS";
        public const string LOYALTY_OF = SHELL_PREFIX + "LOYALTY_OF";
        public const string LOYALTY_STEPS = SHELL_PREFIX + "LOYALTY_STEPS";
        public const string LOYALTY_READY = SHELL_PREFIX + "LOYALTY_READY";
        public const string LOYALTY_UNTIL = SHELL_PREFIX + "LOYALTY_UNTIL";
        public const string LOYALTY_DAYS_COUNT = SHELL_PREFIX + "LOYALTY_DAYS_COUNT";
        public const string LOYALTY_POINTS_COUNT = SHELL_PREFIX + "LOYALTY_POINTS_COUNT";
        public const string LOYALTY_COMPLETED_ON = SHELL_PREFIX + "LOYALTY_COMPLETED_ON";
        public const string LOYALTY_REMAINING = SHELL_PREFIX + "LOYALTY_REMAINING";
        public const string LOYALTY_REWARD_WAITING = SHELL_PREFIX + "LOYALTY_REWARD_WAITING";
        public const string LOYALTY_REWARD_WAITING_SHORT = SHELL_PREFIX + "LOYALTY_REWARD_WAITING_SHORT";
        public const string LOYALTY_REWARD_GIFT = SHELL_PREFIX + "LOYALTY_REWARD_GIFT";
        public const string LOYALTY_POOL_LEFT = SHELL_PREFIX + "LOYALTY_POOL_LEFT";
        public const string LOYALTY_STATE_ENDED = SHELL_PREFIX + "LOYALTY_STATE_ENDED";
        public const string LOYALTY_STATE_DONE = SHELL_PREFIX + "LOYALTY_STATE_DONE";
        public const string LOYALTY_EARNED = SHELL_PREFIX + "LOYALTY_EARNED";
        public const string LOYALTY_EARNED_TIMES = SHELL_PREFIX + "LOYALTY_EARNED_TIMES";
        public const string LOYALTY_SECRET = SHELL_PREFIX + "LOYALTY_SECRET";
        public const string LOYALTY_SECRET_HINT = SHELL_PREFIX + "LOYALTY_SECRET_HINT";
        public const string LOYALTY_HINT_TITLE = SHELL_PREFIX + "LOYALTY_HINT_TITLE";
        public const string LOYALTY_HINT_RING = SHELL_PREFIX + "LOYALTY_HINT_RING";
        public const string LOYALTY_NEWS_LEVEL_UP = SHELL_PREFIX + "LOYALTY_NEWS_LEVEL_UP";
        public const string LOYALTY_NEWS_LEVEL_DOWN = SHELL_PREFIX + "LOYALTY_NEWS_LEVEL_DOWN";
        public const string LOYALTY_NEWS_ACHIEVEMENT = SHELL_PREFIX + "LOYALTY_NEWS_ACHIEVEMENT";
        public const string LOYALTY_NEWS_CHALLENGE = SHELL_PREFIX + "LOYALTY_NEWS_CHALLENGE";
        public const string LOYALTY_NEWS_REWARD_WAITING = SHELL_PREFIX + "LOYALTY_NEWS_REWARD_WAITING";
        public const string LOYALTY_NEWS_REWARD_GIVEN = SHELL_PREFIX + "LOYALTY_NEWS_REWARD_GIVEN";
        public const string LOYALTY_EMPTY = SHELL_PREFIX + "LOYALTY_EMPTY";
        public const string LOYALTY_LAST_LEVEL_CHANGE = SHELL_PREFIX + "LOYALTY_LAST_LEVEL_CHANGE";

        #endregion

        // key -> (two-letter culture -> text). Plural forms are separated by '|' in the
        // order the culture's rule returns; see PluralIndex.
        private static readonly Dictionary<string, Dictionary<string, string>> _overrides =
            new()
            {
                #region VENDOR KEYS REWORDED

                // Shorter than "Добавить в корзину": the button on the home board and the
                // bar row are icon-width, and the full phrase does not fit.
                [GEN_ADD_TO_CART] = new() { ["ru"] = "В корзину" },
                [GEN_ALL] = new() { ["ru"] = "Все" },
                // Captions on the account page's time rows: a stat label, not a table header.
                [TIME_REMAINING] = new() { ["ru"] = "Осталось", ["en"] = "Left" },
                [TIME_USABLE_HERE] = new() { ["ru"] = "На этом ПК", ["en"] = "On this PC" },
                [TIME_IN_CREDIT] = new() { ["ru"] = "В кредит", ["en"] = "On credit" },
                [TIME_BOUGHT] = new() { ["ru"] = "Куплен", ["en"] = "Bought" },
                [GEN_BALANCE] = new() { ["ru"] = "На счете" },
                [LOGOUT] = new() { ["ru"] = "Выйти" },
                [SEARCH_APPS] = new() { ["ru"] = "Игры и программы" },
                [SEARCH_SHOP] = new() { ["ru"] = "Бар и магазин" },
                [SEARCH_SHOW_ALL] = new() { ["ru"] = "Показать все" },
                [ACTIVE_APPS_TITLE] = new() { ["ru"] = "Мои приложения", ["en"] = "My applications" },
                [NOTIFICATIONS_EMPTY] = new() { ["ru"] = "Новых уведомлений нет" },
                [NOTIFICATIONS_CLEAR] = new() { ["ru"] = "Очистить список", ["en"] = "Clear the list" },
                [DEPOSIT_QUICK_SELECT] = new() { ["ru"] = "Выберите сумму", ["en"] = "Choose an amount" },
                [DEPOSIT_OR_ENTER_AMOUNT] = new() { ["ru"] = "Или впишите свою", ["en"] = "Or enter your own" },
                [DEPOSIT_WILL_BE_CREDITED] = new() { ["ru"] = "Зачислим на счёт", ["en"] = "Will be credited" },
                [DEPOSIT_TO_BE_PAID] = new() { ["ru"] = "К оплате", ["en"] = "To pay" },
                [DEPOSIT_QR_CODE] = new() { ["ru"] = "Наведите камеру телефона", ["en"] = "Point your phone camera at it" },
                [DEPOSIT_PAY_FROM_PC] = new() { ["ru"] = "Открыть в браузере", ["en"] = "Open in a browser" },
                [DEPOSIT_CLEAR] = new() { ["ru"] = "Сбросить QR", ["en"] = "Reset the QR code" },
                [DEPOSIT_OR] = new() { ["ru"] = "или", ["en"] = "or" },

                #endregion

                #region SHELL KEYS

                [HERO_EMPTY] = new() { ["ru"] = "Здесь пока ничего нет", ["en"] = "Nothing here yet" },
                [HERO_PROMOS] = new() { ["ru"] = "Акции", ["en"] = "Promotions" },
                [HERO_POPULAR] = new() { ["ru"] = "Популярное", ["en"] = "Popular" },
                [HOME_GAMES_SECTION] = new() { ["ru"] = "Игры и приложения", ["en"] = "Games and applications" },
                [HOME_TIME_PACKAGES] = new() { ["ru"] = "Пакеты времени", ["en"] = "Time packages" },
                [HOME_NO_PACKAGES] = new() { ["ru"] = "Сейчас нет пакетов, доступных к покупке", ["en"] = "No packages are available to buy right now" },
                [HOME_BAR] = new() { ["ru"] = "Бар", ["en"] = "Bar" },
                [HOME_WHOLE_SHOP] = new() { ["ru"] = "Весь магазин", ["en"] = "Whole shop" },

                [GEN_EMPTY] = new() { ["ru"] = "Пока пусто", ["en"] = "Nothing here yet" },
                [GEN_BUY] = new() { ["ru"] = "Купить", ["en"] = "Buy" },
                [GEN_WORKING] = new() { ["ru"] = "Минуту…", ["en"] = "One moment…" },
                [GEN_DONE] = new() { ["ru"] = "Готово", ["en"] = "Done" },
                [GEN_LATER] = new() { ["ru"] = "Позже", ["en"] = "Later" },
                [GEN_HIDE] = new() { ["ru"] = "Скрыть", ["en"] = "Hide" },
                [GEN_TO_SHOP] = new() { ["ru"] = "В магазин", ["en"] = "To the shop" },

                [DEPOSIT_SUCCESS_TITLE] = new() { ["ru"] = "Успешно", ["en"] = "Done" },
                [DEPOSIT_SUCCESS_TEXT] = new() { ["ru"] = "Счёт пополнен на {0}", ["en"] = "{0} added to your account" },
                [DEPOSIT_SUCCESS_HINT] = new() { ["ru"] = "Окно закроется автоматически через 5 секунд", ["en"] = "This will close automatically in 5 seconds" },
                [DEPOSIT_PAGE_TITLE] = new() { ["ru"] = "Пополнение", ["en"] = "Top up" },

                [BUY_TITLE] = new() { ["ru"] = "Покупка пакета", ["en"] = "Buy a package" },
                [BUY_TITLE_TOPUP] = new() { ["ru"] = "Пополнение счёта", ["en"] = "Top up your account" },
                [BUY_DONE_TITLE] = new() { ["ru"] = "Пакет куплен", ["en"] = "Package bought" },
                // Stands in when the product itself has not loaded, so it has to name the
                // thing in full - the vendor's "Пакет" is a column heading, not a name.
                [BUY_FALLBACK_NAME] = new() { ["ru"] = "Пакет времени", ["en"] = "Time package" },
                [BUY_SHORTFALL] = new() { ["ru"] = "Не хватает", ["en"] = "Short by" },
                [BUY_DISCOUNT] = new() { ["ru"] = "Скидка", ["en"] = "Discount" },
                [BUY_FREE] = new() { ["ru"] = "Оплата не требуется", ["en"] = "No payment needed" },
                [BUY_TOP_UP_BY] = new() { ["ru"] = "Пополнить на {0}", ["en"] = "Top up by {0}" },
                [BUY_TOP_UP_AT_COUNTER] = new() { ["ru"] = "Пополните на стойке", ["en"] = "Top up at the counter" },
                [BUY_TOPUP_SUB] = new() { ["ru"] = "Не хватает {0} — после пополнения вернёмся к покупке", ["en"] = "Short by {0} - the purchase continues once the money is in" },
                // Caption over the pay chooser, and over the line that replaces it when the
                // club offers exactly one way.
                [BUY_PAY_WITH] = new() { ["ru"] = "Способ оплаты", ["en"] = "Payment method" },
                [BUY_WAY_BALANCE] = new() { ["ru"] = "Со счёта", ["en"] = "From the balance" },
                [BUY_WAY_POINTS] = new() { ["ru"] = "Баллами", ["en"] = "With points" },
                [BUY_WAY_COUNTER_HINT] = new() { ["ru"] = "Оплата у администратора", ["en"] = "Paid at the counter" },
                [BUY_COUNTER_NOTE] = new() { ["ru"] = "Заказ оплачивается у администратора", ["en"] = "The order is paid for at the counter" },
                [BUY_ON_ACCOUNT] = new() { ["ru"] = "На счёте {0}", ["en"] = "{0} on the account" },
                [BUY_YOU_HAVE] = new() { ["ru"] = "У вас {0}", ["en"] = "You have {0}" },
                [BUY_OTHER_ITEMS] = new() { ["ru"] = "Товары в корзине", ["en"] = "Goods in the cart" },
                [BUY_FEES] = new() { ["ru"] = "Налоги и сборы", ["en"] = "Taxes and fees" },
                [BUY_LEFT_AFTER] = new() { ["ru"] = "Останется на счёте", ["en"] = "Left on the account" },
                [BUY_POINTS_LEFT_AFTER] = new() { ["ru"] = "Останется", ["en"] = "Left afterwards" },
                [BUY_BUY_FOR] = new() { ["ru"] = "Купить за {0}", ["en"] = "Buy for {0}" },
                [BUY_GET_FREE] = new() { ["ru"] = "Получить", ["en"] = "Get it" },
                [BUY_ORDERED_TITLE] = new() { ["ru"] = "Заказ оформлен", ["en"] = "Order placed" },
                [BUY_ORDERED_HINT] = new() { ["ru"] = "Оплатите его у администратора", ["en"] = "Pay for it at the counter" },
                [BUY_FAILED_TITLE] = new() { ["ru"] = "Не получилось", ["en"] = "That did not work" },
                // The segment that stands for every method settled with the staff, and the
                // note under it before one of them is chosen.
                [BUY_AT_COUNTER] = new() { ["ru"] = "На кассе", ["en"] = "At the counter" },
                [BUY_AT_COUNTER_NOTE] = new() { ["ru"] = "у администратора", ["en"] = "with the staff" },
                // The shop checkout's button and result.
                [BUY_ORDER_FOR] = new() { ["ru"] = "Заказать за {0}", ["en"] = "Order for {0}" },
                [BUY_ORDER_PAID_TITLE] = new() { ["ru"] = "Заказ оплачен", ["en"] = "Order paid" },
                [BUY_ITEMS_COUNT] = new()
                {
                    ["ru"] = "{0} товар|{0} товара|{0} товаров",
                    ["en"] = "{0} item|{0} items",
                },
                [BUY_NO_WAYS] = new() { ["ru"] = "Оплата с этого компьютера недоступна — обратитесь к администратору", ["en"] = "Paying from this PC is not available - please ask the staff" },
                [BUY_POINTS_COUNT] = new()
                {
                    ["ru"] = "{0:N0} балл|{0:N0} балла|{0:N0} баллов",
                    ["en"] = "{0:N0} point|{0:N0} points",
                },

                [LOGIN_PIN_PLACEHOLDER] = new() { ["ru"] = "код из подтверждения", ["en"] = "code from your confirmation" },

                [RESV_PC_RESERVED] = new() { ["ru"] = "Компьютер забронирован", ["en"] = "This PC is reserved" },
                [RESV_DEVICE_RESERVED] = new() { ["ru"] = "Устройство забронировано", ["en"] = "This machine is reserved" },
                [RESV_FROM] = new() { ["ru"] = "с {0}", ["en"] = "from {0}" },
                [RESV_IN] = new() { ["ru"] = "через {0}", ["en"] = "in {0}" },
                [RESV_NOTIF_TITLE_CODE] = new() { ["ru"] = "Этот компьютер забронирован", ["en"] = "This PC is reserved" },
                [RESV_NOTIF_TITLE_PAY] = new() { ["ru"] = "Бронь ждёт оплаты", ["en"] = "The reservation is awaiting payment" },
                [RESV_NOTIF_MSG_CODE] = new() { ["ru"] = "Если бронировали вы, введите код бронирования, чтобы продолжить сеанс.", ["en"] = "If the reservation is yours, enter its code to carry on." },
                [RESV_NOTIF_MSG_PAY] = new() { ["ru"] = "Бронирование подтверждено. Осталось оплатить — иначе сеанс закроется.", ["en"] = "The reservation is confirmed. Pay for it, or the session will end." },
                [RESV_ENTER_CODE] = new() { ["ru"] = "Ввести код", ["en"] = "Enter the code" },

                [GRACE_RESERVED_AT] = new() { ["ru"] = "Это устройство забронировано на {0}. ", ["en"] = "This machine is reserved from {0}. " },
                [GRACE_RESERVED_HINT] = new() { ["ru"] = "Если бронь ваша — введите код бронирования. Если нет, обратитесь к администратору.", ["en"] = "If the reservation is yours, enter its code. If it is not, please ask the staff." },
                [GRACE_CONFIRMED] = new() { ["ru"] = "Бронирование подтверждено", ["en"] = "Reservation confirmed" },
                [GRACE_CONFIRMED_UNPAID] = new() { ["ru"] = "Бронирование подтверждено, но требует оплаты.", ["en"] = "The reservation is confirmed but still needs paying for." },
                [GRACE_PIN_LABEL] = new() { ["ru"] = "Код бронирования", ["en"] = "Reservation code" },
                [GRACE_TIME_LEFT] = new() { ["ru"] = "До выхода осталось", ["en"] = "Time before sign-out" },
                [GRACE_TIME_UP] = new() { ["ru"] = "Время вышло", ["en"] = "Your time is up" },
                [GRACE_TIME_UP_MESSAGE] = new()
                {
                    ["ru"] = "Сессия ещё активна. Пополните счёт, чтобы продолжить с того же места — ничего не потеряется.",
                    ["en"] = "The session is still running. Top up to carry on from where you left off - nothing is lost.",
                },
                [GRACE_TOP_UP_ACCOUNT] = new() { ["ru"] = "Пополнить счёт", ["en"] = "Top up the account" },
                [GEN_CONFIRM] = new() { ["ru"] = "Подтвердить", ["en"] = "Confirm" },
                [GRACE_PIN_WRONG] = new() { ["ru"] = "Код не подошёл. Проверьте его в подтверждении брони.", ["en"] = "That code did not work. Check it in your reservation confirmation." },
                [GRACE_PIN_FAILED] = new() { ["ru"] = "Не получилось проверить код. Попробуйте ещё раз.", ["en"] = "The code could not be checked. Please try again." },

                [DEPLOY_TITLE] = new() { ["ru"] = "Развёртывание", ["en"] = "Deploying" },
                [DEPLOY_TITLE_PREPARING] = new() { ["ru"] = "Подготовка", ["en"] = "Preparing" },
                [DEPLOY_COPYING] = new() { ["ru"] = "Копируем файлы игры", ["en"] = "Copying the game files" },
                [DEPLOY_LAUNCHING] = new() { ["ru"] = "Запускаем: {0}", ["en"] = "Starting: {0}" },
                [DEPLOY_READY_COUNT] = new()
                {
                    ["ru"] = "{0} приложение готово|{0} приложения готовы|{0} приложений готовы",
                    ["en"] = "{0} application ready|{0} applications ready",
                },
                [DEPLOY_RUNNING_COUNT] = new()
                {
                    ["ru"] = "{0} приложение · открыть список|{0} приложения · открыть список|{0} приложений · открыть список",
                    ["en"] = "{0} application · open the list|{0} applications · open the list",
                },
                [DEPLOY_OPEN_APPS] = new() { ["ru"] = "Открыть «Мои приложения»", ["en"] = "Open My applications" },

                [ACTIVE_APPS_HINT] = new() { ["ru"] = "Управление запущенными приложениями", ["en"] = "Manage what is running" },
                [ACTIVE_APPS_EMPTY_HINT] = new() { ["ru"] = "Запустите игру из каталога — она появится здесь", ["en"] = "Start a game from the catalogue and it will show up here" },

                [NOTIFICATIONS_HINT] = new() { ["ru"] = "Всё, что мы вам присылали", ["en"] = "Everything we have sent you" },
                [NOTIFICATIONS_EMPTY_TITLE] = new() { ["ru"] = "Тишина", ["en"] = "All quiet" },

                [TIME_TOOLTIP_TITLE] = new() { ["ru"] = "Ваше время", ["en"] = "Your time" },
                [TIME_TOOLTIP_ALL_PACKAGES] = new() { ["ru"] = "Все пакеты", ["en"] = "All packages" },
                [TIME_TOOLTIP_LEFT] = new() { ["ru"] = "Осталось", ["en"] = "Left" },
                [TIME_TOOLTIP_SPENDABLE] = new() { ["ru"] = "Можно потратить", ["en"] = "Available to spend" },
                [TIME_TOOLTIP_UNLIMITED] = new() { ["ru"] = "Время не ограничено", ["en"] = "Time is not limited" },
                [TIME_TOOLTIP_MONEY_LOW] = new() { ["ru"] = "Списывается со счёта, пока вы играете", ["en"] = "Charged from the account while you play" },
                [TIME_TOOLTIP_ENDING] = new() { ["ru"] = "Время скоро закончится", ["en"] = "Your time is about to run out" },
                [TIME_TOOLTIP_CTA_BOTH] = new() { ["ru"] = "Чтобы продолжить — пополните счёт или возьмите пакет времени", ["en"] = "To carry on, top up or take a time package" },
                [TIME_TOOLTIP_CTA_PACKAGE] = new() { ["ru"] = "Чтобы продолжить — возьмите пакет времени", ["en"] = "To carry on, take a time package" },
                [TIME_TOOLTIP_CTA_TOPUP] = new() { ["ru"] = "Чтобы продолжить — пополните счёт", ["en"] = "To carry on, top up your account" },
                [TIME_TOOLTIP_CREDIT] = new() { ["ru"] = "Вам доступен кредит времени", ["en"] = "You have time credit available" },

                // {0} hours, {1} minutes. Kept as this shell's own rather than the vendor's
                // "{0} h {1} min", so the wording in the reservation tile and the purchase
                // dialog stays the one the clubs already read.
                [DURATION_HOURS_MINUTES] = new() { ["ru"] = "{0} ч {1} мин", ["en"] = "{0} h {1} min" },
                [DURATION_HOURS] = new() { ["ru"] = "{0} ч", ["en"] = "{0} h" },
                [DURATION_MINUTES] = new() { ["ru"] = "{0} мин", ["en"] = "{0} min" },

                [TIME_TOOLTIP_EMPTY] = new() { ["ru"] = "Активных пакетов нет", ["en"] = "No active time packages" },

                // The account page: one head, three tabs.
                [ACCOUNT_TAB_PROFILE] = new() { ["ru"] = "Профиль", ["en"] = "Profile" },
                [ACCOUNT_TAB_TIME] = new() { ["ru"] = "Время", ["en"] = "Time" },
                [ACCOUNT_MEMBER_SINCE] = new() { ["ru"] = "В клубе с {0}", ["en"] = "Member since {0}" },
                [ACCOUNT_NOT_SET] = new() { ["ru"] = "Не указан", ["en"] = "Not set" },
                [ACCOUNT_CONTACTS_NOTE] = new() { ["ru"] = "Изменить контакты может администратор клуба", ["en"] = "The club staff can change your contact details" },
                [ACCOUNT_PASSWORD_NOTE] = new() { ["ru"] = "Пароль для входа на любом ПК клуба", ["en"] = "The password you sign in with on any club PC" },
                [ACCOUNT_NO_TIME] = new() { ["ru"] = "Пакетов времени нет", ["en"] = "No time packages" },
                [ACCOUNT_NO_TIME_HINT] = new() { ["ru"] = "Купленные пакеты появятся здесь", ["en"] = "Packages you buy will show up here" },
                [ACCOUNT_NO_PURCHASES] = new() { ["ru"] = "Покупок пока нет", ["en"] = "No purchases yet" },
                [ACCOUNT_NO_PURCHASES_HINT] = new() { ["ru"] = "Здесь будет всё, что вы заказывали", ["en"] = "Everything you order will be listed here" },
                [ACCOUNT_TIME_HINT] = new() { ["ru"] = "Пакеты расходуются по очереди, сверху вниз", ["en"] = "Packages are spent in order, top to bottom" },
                [ACCOUNT_ACTIVE_NOW] = new() { ["ru"] = "Идёт сейчас", ["en"] = "Active now" },
                [ACCOUNT_QUEUE_POSITION] = new() { ["ru"] = "{0}-й в очереди", ["en"] = "#{0} in line" },
                [ACCOUNT_NOT_HERE] = new() { ["ru"] = "Не на этом ПК", ["en"] = "Not on this PC" },
                [ACCOUNT_ORDER_NUMBER] = new() { ["ru"] = "Заказ №{0}", ["en"] = "Order #{0}" },
                [ACCOUNT_NEWER] = new() { ["ru"] = "Новее", ["en"] = "Newer" },
                [ACCOUNT_OLDER] = new() { ["ru"] = "Старее", ["en"] = "Older" },
                [ACCOUNT_EXPIRES_NEVER] = new() { ["ru"] = "Не сгорает", ["en"] = "Does not expire" },

                // Product page.
                [PD_INCLUDES] = new() { ["ru"] = "В комплекте", ["en"] = "Includes" },
                [PD_WHERE] = new() { ["ru"] = "Где действует", ["en"] = "Where it works" },
                [PD_ABOUT] = new() { ["ru"] = "Описание", ["en"] = "About" },
                [PD_NO_DESCRIPTION] = new() { ["ru"] = "Без описания", ["en"] = "No description" },
                [PD_UNAVAILABLE] = new() { ["ru"] = "Сейчас недоступно", ["en"] = "Not available right now" },
                [PD_EXPIRES_AT_DAYTIME] = new() { ["ru"] = "Сгорает в {0}", ["en"] = "Expires at {0}" },
                [PD_EXPIRES_AFTER] = new() { ["ru"] = "Сгорает через {0}", ["en"] = "Expires after {0}" },
                [PD_EXPIRES_AT_LOGOUT] = new() { ["ru"] = "Сгорает при выходе", ["en"] = "Expires on sign-out" },
                [PD_QUANTITY] = new() { ["ru"] = "Количество", ["en"] = "Quantity" },

                // Loyalty (the ladder, achievements and challenges of Gizmo 3.0.95+)
                [LOYALTY_TAB] = new() { ["ru"] = "Прогресс", ["en"] = "Progress" },
                [LOYALTY_HOME_CAP] = new() { ["ru"] = "Ваш прогресс", ["en"] = "Your progress" },
                [LOYALTY_MORE] = new() { ["ru"] = "Мой прогресс", ["en"] = "My progress" },
                [LOYALTY_LEVEL_CAP] = new() { ["ru"] = "Ваш уровень", ["en"] = "Your level" },
                [LOYALTY_STATE_EARNING] = new() { ["ru"] = "Набираете", ["en"] = "Earning" },
                [LOYALTY_STATE_SECURED] = new() { ["ru"] = "Закреплён", ["en"] = "Secured" },
                [LOYALTY_STATE_AWAITING] = new() { ["ru"] = "Ждёт итогов", ["en"] = "Awaiting results" },
                [LOYALTY_RETAIN_SHORT] = new() { ["ru"] = "Ещё {0}, чтобы удержать", ["en"] = "{0} more to keep it" },
                [LOYALTY_RETAIN_LINE] = new() { ["ru"] = "Наберите ещё {0} до {1} — и уровень останется.", ["en"] = "Earn {0} more by {1} and the level stays." },
                [LOYALTY_NEXT_SHORT] = new() { ["ru"] = "Ещё {0} до уровня {1}", ["en"] = "{0} more to {1}" },
                [LOYALTY_NEXT_LINE] = new() { ["ru"] = "Следующий уровень — {0}: ещё {1}.", ["en"] = "Next level - {0}: {1} more." },
                [LOYALTY_SECURED_SHORT] = new() { ["ru"] = "Закреплён до {0}", ["en"] = "Secured until {0}" },
                [LOYALTY_SECURED_LINE] = new() { ["ru"] = "Уровень закреплён до {0}.", ["en"] = "The level is secured until {0}." },
                [LOYALTY_TOP_LINE] = new() { ["ru"] = "Это высший уровень, и он закреплён до {0}.", ["en"] = "This is the top level, and it is secured until {0}." },
                [LOYALTY_AWAITING_LINE] = new() { ["ru"] = "Итоги подводятся {0}.", ["en"] = "Results are settled on {0}." },
                [LOYALTY_REQUIREMENTS_LINE] = new() { ["ru"] = "Уровень {0} даётся за достижения: {1}.", ["en"] = "{0} is earned with achievements: {1}." },
                [LOYALTY_POINTS_PERIOD] = new() { ["ru"] = "Очки {0}", ["en"] = "Points {0}" },
                [LOYALTY_PERIOD_DAY] = new() { ["ru"] = "за сегодня", ["en"] = "today" },
                [LOYALTY_PERIOD_WEEK] = new() { ["ru"] = "за неделю", ["en"] = "this week" },
                [LOYALTY_PERIOD_MONTH] = new() { ["ru"] = "за месяц", ["en"] = "this month" },
                [LOYALTY_PERIOD_QUARTER] = new() { ["ru"] = "за квартал", ["en"] = "this quarter" },
                [LOYALTY_PERIOD_YEAR] = new() { ["ru"] = "за год", ["en"] = "this year" },
                [LOYALTY_SETTLE] = new() { ["ru"] = "Итоги", ["en"] = "Results" },
                [LOYALTY_START] = new() { ["ru"] = "старт", ["en"] = "start" },
                [LOYALTY_KEEP_AT] = new() { ["ru"] = "удержать", ["en"] = "keep" },
                [LOYALTY_BONUSES] = new() { ["ru"] = "Ваши бонусы:", ["en"] = "Your perks:" },
                [LOYALTY_PERK_DISCOUNT] = new() { ["ru"] = "скидка {0}", ["en"] = "{0} off" },
                [LOYALTY_PERK_BONUS] = new() { ["ru"] = "бонус {0}", ["en"] = "{0} bonus" },
                [LOYALTY_PERK_QUEUE] = new() { ["ru"] = "приоритет в очереди", ["en"] = "priority in the queue" },
                [LOYALTY_CHALLENGES] = new() { ["ru"] = "Челленджи", ["en"] = "Challenges" },
                [LOYALTY_ACHIEVEMENTS] = new() { ["ru"] = "Достижения", ["en"] = "Achievements" },
                [LOYALTY_OF] = new() { ["ru"] = "{0} из {1}", ["en"] = "{0} of {1}" },
                [LOYALTY_STEPS] = new() { ["ru"] = "шагов", ["en"] = "steps" },
                [LOYALTY_READY] = new() { ["ru"] = "готово", ["en"] = "done" },
                [LOYALTY_UNTIL] = new() { ["ru"] = "до {0}", ["en"] = "until {0}" },
                [LOYALTY_DAYS_COUNT] = new() { ["ru"] = "{0} день|{0} дня|{0} дней", ["en"] = "{0} day|{0} days" },
                [LOYALTY_POINTS_COUNT] = new() { ["ru"] = "{0} очко|{0} очка|{0} очков", ["en"] = "{0} point|{0} points" },
                [LOYALTY_COMPLETED_ON] = new() { ["ru"] = "выполнен {0}", ["en"] = "completed {0}" },
                [LOYALTY_REMAINING] = new() { ["ru"] = "Осталось: {0}", ["en"] = "Left: {0}" },
                [LOYALTY_REWARD_WAITING] = new() { ["ru"] = "Награда ждёт вас у стойки — покажите администратору эту вкладку.", ["en"] = "Your reward is waiting at the counter - show the staff this tab." },
                [LOYALTY_REWARD_WAITING_SHORT] = new() { ["ru"] = "Награда ждёт у стойки", ["en"] = "Reward waiting at the counter" },
                [LOYALTY_REWARD_GIFT] = new() { ["ru"] = "Подарок", ["en"] = "Gift" },
                [LOYALTY_POOL_LEFT] = new() { ["ru"] = "Осталось {0} на всех", ["en"] = "{0} left for everyone" },
                [LOYALTY_STATE_ENDED] = new() { ["ru"] = "Завершён", ["en"] = "Ended" },
                [LOYALTY_STATE_DONE] = new() { ["ru"] = "Выполнено", ["en"] = "Done" },
                [LOYALTY_EARNED] = new() { ["ru"] = "Получено", ["en"] = "Earned" },
                [LOYALTY_EARNED_TIMES] = new() { ["ru"] = "Получено ×{0}", ["en"] = "Earned ×{0}" },
                [LOYALTY_SECRET] = new() { ["ru"] = "Секретное", ["en"] = "Secret" },
                [LOYALTY_SECRET_HINT] = new() { ["ru"] = "Откроется, когда получите", ["en"] = "Revealed when you earn it" },
                [LOYALTY_HINT_TITLE] = new() { ["ru"] = "{0} — ваш уровень", ["en"] = "{0} - your level" },
                [LOYALTY_HINT_RING] = new() { ["ru"] = "Кольцо на аватаре — ваш прогресс", ["en"] = "The ring on your avatar is your progress" },
                [LOYALTY_NEWS_LEVEL_UP] = new() { ["ru"] = "Новый уровень", ["en"] = "New level" },
                [LOYALTY_NEWS_LEVEL_DOWN] = new() { ["ru"] = "Уровень изменился", ["en"] = "Level changed" },
                [LOYALTY_NEWS_ACHIEVEMENT] = new() { ["ru"] = "Достижение получено", ["en"] = "Achievement earned" },
                [LOYALTY_NEWS_CHALLENGE] = new() { ["ru"] = "Челлендж выполнен", ["en"] = "Challenge completed" },
                [LOYALTY_NEWS_REWARD_WAITING] = new() { ["ru"] = "Награда ждёт у стойки", ["en"] = "Reward waiting at the counter" },
                [LOYALTY_NEWS_REWARD_GIVEN] = new() { ["ru"] = "Награда получена", ["en"] = "Reward received" },
                [LOYALTY_EMPTY] = new() { ["ru"] = "Пока нечего показать", ["en"] = "Nothing to show yet" },
                [LOYALTY_LAST_LEVEL_CHANGE] = new() { ["ru"] = "{0} — уровень {1}", ["en"] = "{0} - level {1}" },

                #endregion

                #region VENDOR KEYS REWORDED (authentication flow)

                ["GIZ_PASSWORD_RECOVERY_ENTER_EMAIL"] = new()
                {
                    ["ru"] = "Введите e-mail, привязанный к вашей учётной записи",
                    ["en"] = "Enter the e-mail linked to your account",
                },
                ["GIZ_PASSWORD_RECOVERY_ENTER_PHONE_NUMBER"] = new()
                {
                    ["ru"] = "Введите номер телефона, привязанный к вашей учётной записи",
                    ["en"] = "Enter the phone number linked to your account",
                },
                ["GIZ_PASSWORD_RECOVERY_PLEASE_ENTER_RECOVERY_CODE"] = new()
                {
                    ["ru"] = "Мы отправили код в SMS на {0}. Введите его ниже.",
                    ["en"] = "We sent a code by text to {0}. Enter it below.",
                },
                ["GIZ_PASSWORD_RECOVERY_SET_PASSWORD_MESSAGE"] = new()
                {
                    ["ru"] = "Почти готово — осталось придумать новый пароль.",
                    ["en"] = "Almost there — just pick a new password.",
                },
                ["GIZ_PASSWORD_RECOVERY_NO_VALID_MOBILE"] = new()
                {
                    ["ru"] = "К этому аккаунту не привязан номер телефона.",
                    ["en"] = "This account has no phone number linked to it.",
                },
                ["GIZ_PASSWORD_RECOVERY_PASSWORD_RESET_FAILED_MESSAGE"] = new()
                {
                    ["ru"] = "Не удалось сбросить пароль. Попробуйте ещё раз.",
                    ["en"] = "Couldn't reset the password. Please try again.",
                },
                ["GIZ_REGISTRATION_MOBILE_CONFIRMATION_MESSAGE"] = new()
                {
                    ["ru"] = "Проверьте код страны и введите номер телефона — мы отправим на него код подтверждения.",
                    ["en"] = "Check your country code and enter your phone number — we'll text you a confirmation code.",
                },
                // The QR screen heading was built from "Sign in with {0}" plus a provider
                // name the club chooses itself, which produced sentences like "Sign in with
                // Get a code in Telegram". The provider name alone already reads as an
                // action.
                ["GIZ_REGISTRATION_REDIRECT_TITLE"] = new()
                {
                    ["ru"] = "{0}",
                    ["en"] = "{0}",
                },
                // The vendor subtitle does not describe this screen, which carries a QR
                // code and its own hint. Empty, so the block is hidden by style.
                ["GIZ_REGISTRATION_REDIRECT_SUBTITLE"] = new()
                {
                    ["ru"] = "",
                    ["en"] = "",
                },
                ["GIZ_REGISTRATION_EMAIL_CONFIRMATION_MESSAGE"] = new()
                {
                    ["ru"] = "Введите e-mail — мы отправим на него код подтверждения.",
                    ["en"] = "Enter your e-mail — we'll send a confirmation code to it.",
                },
                ["GIZ_REGISTRATION_FAILED_MESSAGE"] = new()
                {
                    ["ru"] = "Что-то пошло не так. Обратитесь к администратору клуба.",
                    ["en"] = "Something went wrong. Please ask the club staff for help.",
                },
                ["GIZ_REGISTRATION_VE_MOBILE_PHONE_USED"] = new()
                {
                    ["ru"] = "Этот номер телефона уже занят.",
                    ["en"] = "That phone number is already taken.",
                },
                ["GIZ_REGISTRATION_VE_EMAIL_ADDRESS_USED"] = new()
                {
                    ["ru"] = "Этот e-mail уже занят.",
                    ["en"] = "That e-mail is already taken.",
                },

                // Login screen only - Login.razor's field labels are the only call sites
                // routed through ShellStringOverrides for these two keys. The generic resx
                // text is correct everywhere else; here it merely repeats the segmented
                // toggle directly above the field.
                ["GIZ_GEN_USERNAME"] = new()
                {
                    ["ru"] = "Логин или e-mail",
                    ["en"] = "Username or e-mail",
                },
                ["GIZ_GEN_PHONE_NUMBER"] = new()
                {
                    ["ru"] = "Введите номер",
                    ["en"] = "Enter your number",
                },

                #endregion
            };

        private static ILocalizationService _localizationService;

        /// <summary>
        /// Associates the shell's localization service, at application start.
        /// </summary>
        /// <remarks>
        /// Same pattern the shell already uses for <c>JSRuntimeService</c> and
        /// <c>NavigationService</c> in <c>App.OnInitialized</c>. It exists so the hundred
        /// or so call sites in markup do not each have to inject and pass the service; the
        /// culture itself is read from <see cref="CultureInfo.CurrentUICulture"/> on every
        /// lookup, so switching language at runtime needs nothing here.
        /// <para>
        /// Called from both Blazor roots - <c>App</c> and <c>NotificationsHost</c>, which
        /// is a window of its own. They share a process, so the first one to start serves
        /// the other; a second call with the same service is a no-op.
        /// </para>
        /// </remarks>
        public static void Associate(ILocalizationService localizationService)
        {
            if (localizationService is not null)
                _localizationService = localizationService;
        }

        /// <summary>
        /// Localized text for <paramref name="key"/>, with substitutions applied.
        /// </summary>
        public static string Get(string key, params object[] args) =>
            Format(Get(_localizationService, key), args);

        /// <summary>
        /// Localized text for a key whose wording depends on <paramref name="count"/>.
        /// </summary>
        /// <remarks>
        /// The count is also passed as <c>{0}</c>, which is what every plural string here
        /// needs. See <see cref="PluralIndex"/> for how the form is chosen.
        /// </remarks>
        public static string GetPlural(string key, int count)
        {
            var forms = Get(_localizationService, key).Split('|');
            var index = PluralIndex(count, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            return Format(forms[Math.Min(index, forms.Length - 1)], count);
        }

        /// <summary>
        /// Which plural form a count takes, as an index into the '|' separated forms.
        /// </summary>
        /// <remarks>
        /// Russian and its neighbours need three forms; the two-form rule covers English
        /// and is a reasonable default for anything else the client is translated into.
        /// A language whose entry supplies fewer forms than the index simply uses its last.
        /// </remarks>
        private static int PluralIndex(int count, string language)
        {
            if (language is "ru" or "uk" or "be")
            {
                var mod100 = count % 100;
                if (mod100 is >= 11 and <= 14)
                    return 2;

                return (count % 10) switch
                {
                    1 => 0,
                    2 or 3 or 4 => 1,
                    _ => 2,
                };
            }

            return count == 1 ? 0 : 1;
        }

        /// <summary>
        /// Localized text for a key, with substitutions applied.
        /// </summary>
        public static string Get(ILocalizationService localizationService, string key, params object[] args) =>
            Format(Get(localizationService, key), args);

        /// <summary>
        /// Localized text for a key.
        /// </summary>
        public static string Get(ILocalizationService localizationService, string key)
        {
            if (_overrides.TryGetValue(key, out var byCulture))
            {
                var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                if (byCulture.TryGetValue(language, out var text))
                    return text;

                // A shell-owned key has nothing in the resource manager, so a club running
                // any other language would be shown the key itself. English is the fallback.
                if (key.StartsWith(SHELL_PREFIX, StringComparison.Ordinal)
                    && byCulture.TryGetValue("en", out var english))
                    return english;
            }

            return localizationService?.GetString(key) ?? key;
        }

        private static string Format(string format, params object[] args)
        {
            if (args is null || args.Length == 0 || string.IsNullOrEmpty(format))
                return format;

            try
            {
                return string.Format(CultureInfo.CurrentCulture, format, args);
            }
            catch (FormatException)
            {
                // A string with no placeholders, or foreign markup: return it as it is,
                // which beats an empty screen.
                return format;
            }
        }
    }
}
