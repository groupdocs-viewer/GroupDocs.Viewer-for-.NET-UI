using System.Linq;

namespace GroupDocs.Viewer.UI.Core.Configuration
{
    public class Config
    {
        //Client-side config
        internal string DefaultDocument { get; set; } = string.Empty;
        internal int PreloadPageCount { get; set; } = 3;
        internal bool IsPageSelector { get; set; } = true;
        internal bool IsThumbnails { get; set; } = true;
        internal bool IsZoom { get; set; } = true;
        internal bool IsSearch { get; set; } = true;
        internal bool IsShowToolBar { get; set; } = true;
        internal bool IsEnableRightClick { get; set; } = true;
        //Client-side and server-side config
        internal bool IsDownload { get; set; } = true;
        internal bool IsUpload { get; set; } = true;
        internal bool Rewrite { get; set; } = false;
        internal bool IsPrint { get; set; } = true;
        internal bool IsBrowse { get; set; } = true;
        internal bool IsPrintAllowed { get; set; } = true;
        internal bool HtmlMode { get; set; } = true;
        //I18n
        internal bool IsShowLanguageMenu { get; set; } = true;
        internal string DefaultLanguage { get; set; } = "en";
        internal string[] SupportedLanguages { get; set; } = new string[]
        {
            "ar", // ar - العربية
            "ca", // ca-ES - Català
            "cs", // cs-CZ - Čeština
            "da", // da-DK - Dansk
            "de", // de-DE - Deutsch
            "el", // el-GR - Ελληνικά
            "en", // en-US - English
            "es", // es-ES - Español
            "fil", // fil-PH - Filipino
            "fr", // fr-FR - Français
            "he", // he-IL - עברית
            "hi", // hi-IN - हिन्दी
            "id", // id-ID - Indonesia
            "it", // it-IT - Italiano
            "ja", // ja-JP - 日本語
            "kk", // kk-KZ - Қазақ Тілі
            "ko", // ko-KR - 한국어
            "ms", // ms-MY - Melayu
            "nl", // nl-NL - Nederlands
            "pl", // pl-PL - Polski
            "pt", // pt-PT - Português
            "ro", // ro-RO - Română
            "ru", // ru-RU - Русский
            "sv", // sv-SE - Svenska
            "vi", // vi-VN - Tiếng Việt
            "th", // th-TH - ไทย
            "tr", // tr-TR - Türkçe
            "uk", // uk-UA - Українська
            "zh-hans", // zh-Hans - 中文(简体)
            "zh-hant", // zh-Hant - 中文(繁體)
        };

        //TODO: Not implemented
        internal bool IsRotate = false;
        internal bool SaveRotateState  = false;

        public Config SetViewerType(ViewerType viewerType)
        {
            HtmlMode = viewerType == ViewerType.HtmlWithExternalResources ||
                       viewerType == ViewerType.HtmlWithEmbeddedResources;
            return this;
        }

        public Config SetPreloadPageCount(int countPages)
        {
            if (countPages < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(countPages), countPages, $"Specified page count '{countPages}' is negative, which is prohibited");
            }
            PreloadPageCount = countPages;
            return this;
        }

        public Config SetDefaultDocument(string filePath)
        {
            DefaultDocument = filePath;
            return this;
        }

        public Config HidePageSelectorControl()
        {
            IsPageSelector = false;
            return this;
        }

        public Config ShowPageSelectorControl()
        {
            IsPageSelector = true;
            return this;
        }

        public Config HideThumbnailsControl()
        {
            IsThumbnails = false;
            return this;
        }
        public Config ShowThumbnailsControl()
        {
            IsThumbnails = true;
            return this;
        }

        public Config DisableFileDownload()
        {
            IsDownload = false;
            return this;
        }
        public Config EnableFileDownload()
        {
            IsDownload = true;
            return this;
        }

        public Config DisableFileUpload()
        {
            IsUpload = false;
            return this;
        }
        public Config EnableFileUpload()
        {
            IsUpload = true;
            return this;
        }

        public Config RewriteFilesOnUpload()
        {
            Rewrite = true;
            return this;
        }

        public Config DisablePrint()
        {
            IsPrint = false;
            IsPrintAllowed = false;
            return this;
        }
        public Config EnablePrint()
        {
            IsPrint = true;
            IsPrintAllowed = true;
            return this;
        }

        public Config DisableFileBrowsing()
        {
            IsBrowse = false;
            return this;
        }
        public Config EnableFileBrowsing()
        {
            IsBrowse = true;
            return this;
        }

        public Config HideZoomButton()
        {
            IsZoom = false;
            return this;
        }
        public Config ShowZoomButton()
        {
            IsZoom = true;
            return this;
        }
        public Config HideSearchControl()
        {
            IsSearch = false;
            return this;
        }
        public Config ShowSearchControl()
        {
            IsSearch = true;
            return this;
        }
        public Config HideToolBar()
        {
            IsShowToolBar = false;
            return this;
        }
        public Config ShowToolBar()
        {
            IsShowToolBar = true;
            return this;
        }


        public Config HidePageRotationControl()
        {
            IsRotate = false;
            return this;
        }
        public Config ShowPageRotationControl()
        {
            IsRotate = true;
            return this;
        }

        public Config DisableRightClick()
        {
            IsEnableRightClick = false;
            return this;
        }

        public Config EnableRightClick()
        {
            IsEnableRightClick = true;
            return this;
        }

        public Config HideLanguageMenu()
        {
            IsShowLanguageMenu = false;
            return this;
        }
        public Config ShowLanguageMenu()
        {
            IsShowLanguageMenu = true;
            return this;
        }

        /// <summary>
        /// Sets default language out of supported:
        /// <see cref="Language.Arabic"/>,
        /// <see cref="Language.Catalan"/>,
        /// <see cref="Language.Czech"/>,
        /// <see cref="Language.Danish"/>,
        /// <see cref="Language.German"/>,
        /// <see cref="Language.Greek"/>,
        /// <see cref="Language.English"/>,
        /// <see cref="Language.Spanish"/>,
        /// <see cref="Language.Filipino"/>,
        /// <see cref="Language.French"/>,
        /// <see cref="Language.Hebrew"/>,
        /// <see cref="Language.Hindi"/>,
        /// <see cref="Language.Indonesian"/>,
        /// <see cref="Language.Italian"/>,
        /// <see cref="Language.Japanese"/>,
        /// <see cref="Language.Kazakh"/>,
        /// <see cref="Language.Korean"/>,
        /// <see cref="Language.Malay"/>,
        /// <see cref="Language.Dutch"/>,
        /// <see cref="Language.Polish"/>,
        /// <see cref="Language.Portuguese"/>,
        /// <see cref="Language.Romanian"/>,
        /// <see cref="Language.Russian"/>,
        /// <see cref="Language.Swedish"/>,
        /// <see cref="Language.Vietnamese"/>,
        /// <see cref="Language.Thai"/>,
        /// <see cref="Language.Turkish"/>,
        /// <see cref="Language.Ukrainian"/>,
        /// <see cref="Language.ChineseSimplified"/>,
        /// <see cref="Language.ChineseTraditional"/>
        /// </summary>
        /// <param name="language">Default language e.g. <see cref="Language.English"/>.</param>
        /// <returns>This config instance.</returns>
        public Config SetDefaultLanguage(Language language)
        {
            DefaultLanguage = language.Code;
            return this;
        }

        /// <summary>
        /// Set supported UI languages. The following languages are supported:
        /// <see cref="Language.Arabic"/>,
        /// <see cref="Language.Catalan"/>,
        /// <see cref="Language.Czech"/>,
        /// <see cref="Language.Danish"/>,
        /// <see cref="Language.German"/>,
        /// <see cref="Language.Greek"/>,
        /// <see cref="Language.English"/>,
        /// <see cref="Language.Spanish"/>,
        /// <see cref="Language.Filipino"/>,
        /// <see cref="Language.French"/>,
        /// <see cref="Language.Hebrew"/>,
        /// <see cref="Language.Hindi"/>,
        /// <see cref="Language.Indonesian"/>,
        /// <see cref="Language.Italian"/>,
        /// <see cref="Language.Japanese"/>,
        /// <see cref="Language.Kazakh"/>,
        /// <see cref="Language.Korean"/>,
        /// <see cref="Language.Malay"/>,
        /// <see cref="Language.Dutch"/>,
        /// <see cref="Language.Polish"/>,
        /// <see cref="Language.Portuguese"/>,
        /// <see cref="Language.Romanian"/>,
        /// <see cref="Language.Russian"/>,
        /// <see cref="Language.Swedish"/>,
        /// <see cref="Language.Vietnamese"/>,
        /// <see cref="Language.Thai"/>,
        /// <see cref="Language.Turkish"/>,
        /// <see cref="Language.Ukrainian"/>,
        /// <see cref="Language.ChineseSimplified"/>,
        /// <see cref="Language.ChineseTraditional"/>
        /// </summary>
        /// <param name="languages">Supported languages.</param>
        /// <returns>This config instance.</returns>
        public Config SetSupportedLanguages(params Language[] languages)
        {
            SupportedLanguages = languages.Select(l => l.Code).ToArray();
            return this;
        }
    }
}
