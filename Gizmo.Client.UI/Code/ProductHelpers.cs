using System;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI
{
    public static class ProductHelpers
    {
        public static string GetProductTimeImage(UserProductViewState product)
        {
            string result = "product-time-default-1.svg";

            if (product != null)
            {
                TimeSpan timeSpan = TimeSpan.FromMinutes(product.TimeProduct.Minutes);

                if (timeSpan.Hours > 12)
                {
                    result = "product-time-default-24.svg";
                }
                else if (timeSpan.Hours > 6)
                {
                    result = "product-time-default-11.svg";
                }
                else if (timeSpan.Hours > 3)
                {
                    result = "product-time-default-4.svg";
                }
            }

            return result;
        }

        public static string GetProductTimeNumber(UserProductViewState product)
        {
            string result = string.Empty;

            if (product != null)
            {
                if (product.TimeProduct.Minutes < 60)
                {
                    result = product.TimeProduct.Minutes.ToString();
                }
                else
                {
                    TimeSpan timeSpan = TimeSpan.FromMinutes(product.TimeProduct.Minutes);
                    result = timeSpan.Hours.ToString();

                    if (timeSpan.Minutes > 0)
                    {
                        result += "+";
                    }
                }
            }

            return result;
        }

        public static string GetProductTimeText(UserProductViewState product, ILocalizationService localizationService)
        {
            string result = string.Empty;

            if (product != null)
            {
                if (product.TimeProduct.Minutes < 60)
                {
                    result = localizationService.GetString("GIZ_TIME_PRODUCT_MINUTES");
                }
                else
                {
                    if (product.TimeProduct.Minutes >= 60 && product.TimeProduct.Minutes < 120)
                    {
                        result = localizationService.GetString("GIZ_TIME_PRODUCT_HOUR");
                    }
                    else
                    {
                        result = localizationService.GetString("GIZ_TIME_PRODUCT_HOURS");
                    }
                }
            }

            return result;
        }

        public static string GetDayTimeFromMinute(int minute)
        {
            TimeSpan timeSpan = TimeSpan.FromMinutes(minute);
            return timeSpan.ToString("hh\\:mm");
        }

        public static string GetExpiresAfterText(UserProductTimeViewState timeProduct, ILocalizationService localizationService)
        {
            string result = string.Empty;

            if (timeProduct != null)
            {
                string expireAfterText = string.Empty;

                switch (timeProduct.ExpireAfterType)
                {
                    case ExpireAfterType.Day:

                        if (timeProduct.ExpiresAfter == 1)
                            expireAfterText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_DAY");
                        else
                            expireAfterText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_DAYS");

                        break;

                    case ExpireAfterType.Hour:

                        if (timeProduct.ExpiresAfter == 1)
                            expireAfterText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_HOUR");
                        else
                            expireAfterText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_HOURS");

                        break;

                    case ExpireAfterType.Minute:

                        if (timeProduct.ExpiresAfter == 1)
                            expireAfterText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_MINUTE");
                        else
                            expireAfterText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_MINUTES");

                        break;
                }

                string expireFromOptionsText = "";

                switch (timeProduct.ExpireFromOptions)
                {
                    case ExpireFromOptionType.Purchase:

                        expireFromOptionsText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_AFTER_PURCHASE");

                        break;

                    case ExpireFromOptionType.Use:

                        expireFromOptionsText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_AFTER_USE");
                        break;
                }

                result = $"{timeProduct.ExpiresAfter} {expireAfterText} {localizationService.GetString("GIN_GEN_OF")} {expireFromOptionsText}"; //TODO: AAA
            }

            return result;
        }

    }
}
