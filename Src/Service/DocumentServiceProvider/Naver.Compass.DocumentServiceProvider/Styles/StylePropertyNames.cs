using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public static class StylePropertyNames
    {
        public const string IS_FIXED_PROP = "IsFixedProp";
        public const string IS_VISIBLE_PROP = "IsVisibleProp";
        public const string X_Prop = "XProp";
        public const string Y_Prop = "YProp";
        public const string HEIGHT_PROP = "HeightProp";
        public const string WIDTH_PROP = "WidthProp";
        public const string Z_PROP = "ZProp";

        public const string WIDGET_ROTATE_PROP = "WidgetRotateProp";
        public const string TEXT_ROTATE_PROP = "TextRotateProp";

        public const string FONT_FAMILY_PROP = "FontFamilyProp";
        public const string FONT_SIZE_PROP = "FontSizeProp";
        public const string BOLD_PROP = "BoldProp";
        public const string ITALIC_PROP = "ItalicProp";
        public const string UNDERLINE_PROP = "UnderlineProp";
        public const string BASELINE_PROP = "BaselineProp";
        public const string OVERLINE_PROP = "OverlineProp";
        public const string STRIKETHROUGH_PROP = "StrikethroughProp";
        public const string FONT_COLOR_PROP = "FontColorProp";
        public const string BULLETED_LIST_PROP = "BulletedListProp";

        public const string LINE_COLOR_PROP = "LineColorProp";
        public const string LINE_WIDTH_PROP = "LineWidthProp";
        public const string LINE_STYLE_PROP = "LineStyleProp";
        public const string ARROW_STYLE_PROP = "ArrowStyleProp";

        public const string CORNER_RADIUS_PROP = "CornerRadiusProp";
        public const string FILL_COLOR_PROP = "FillColorProp";
        public const string OPACITY_PROP = "OpacityProp";
        public const string HORZ_ALIGN_PROP = "HorzAlignProp";
        public const string VERT_ALIGN_PROP = "VertAlignProp";

        //---------------------------------------------------------------
        // TODO : use CSS style property name in feature version.

        // CSS 2.1 - Margin
        public const string MARGIN_TOP = "margin-top";
        public const string MARGIN_RIGHT = "margin-right";
        public const string MARGIN_BOTTOM = "margin-bottom";
        public const string MARGIN_LEFT = "margin-left";
        public const string SHORT_MARGIN = "margin";
    
        // CSS 2.1 - Padding
        public const string PADDING_TOP = "padding-top";
        public const string PADDING_RIGHT = "padding-right";
        public const string PADDING_BOTTOM = "padding-bottom";
        public const string PADDING_LEFT = "padding-left";
        public const string SHORT_PADDING = "padding";
    
        // CSS 2.1 - Border
        public const string BORDER_TOP_WIDTH = "border-top-width";
        public const string BORDER_RIGHT_WIDTH = "border-right-width";
        public const string BORDER_BOTTOM_WIDTH = "border-bottom-width";
        public const string BORDER_LEFT_WIDTH = "border-left-width";
        public const string BORDER_WIDTH = "border-width";
    
        public const string BORDER_TOP_COLOR = "border-top-color";
        public const string BORDER_RIGHT_COLOR = "border-right-color";
        public const string BORDER_BOTTOM_COLOR = "border-bottom-color";
        public const string BORDER_LEFT_COLOR = "border-left-color";
        public const string BORDER_COLOR = "border-color";
    
        public const string BORDER_TOP_STYLE = "border-top-style";
        public const string BORDER_RIGHT_STYLE = "border-right-style";
        public const string BORDER_BOTTOM_STYLE = "border-bottom-style";
        public const string BORDER_LEFT_STYLE = "border-left-style";
        public const string BORDER_STYLE = "border-style";
    
        public const string SHORT_BORDER_TOP = "border-top";
        public const string SHORT_BORDER_RIGHT = "border-right";
        public const string SHORT_BORDER_BUTTOM = "border-bottom";
        public const string SHORT_BORDER_LEFT = "border-left";
        public const string SHORT_BORDER = "border";
    
        // CSS 2.1 - Visual formatting model
        public const string DISPLAY = "display";
        public const string POSITION = "position";
        public const string TOP = "top";
        public const string RIGHT = "right";
        public const string BOTTOM = "bottom";
        public const string LEFT = "left";
        public const string FLOAT = "float";
        public const string CLEAR = "clear";
        public const string Z_INDEX = "z-index";
        public const string DIRECTION = "direction"; 
        public const string UNICODE_BIDI = "unicode-bidi"; 
    
        public const string WIDTH = "width";
        public const string MIN_WIDTH = "min-width"; 
        public const string MAX_WIDTH = "max-width";
        public const string HEIGHT = "height";
        public const string MIN_HEIGHT = "min-height";
        public const string MAX_HEIGHT = "max-height"; 
        public const string LINE_HEIGHT = "line-height"; 
        public const string VERTICAL_ALIGN = "vertical-align"; 
    
        // CSS 2.1 - Visual effects
        public const string OVERFLOW = "overflow";
        public const string CLIP = "clip";
        public const string VISIBILITY = "visibility";
    
        // CSS 2.1 - Generated content, automatic numbering, and lists 
        public const string CONTENT = "content";
        public const string QUOTES = "quotes";
        public const string COUNTER_RESET = "counter-reset";
        public const string COUNTER_INCREMENT = "counter-increment"; 
        public const string LIST_STYLE_TYPE = "list-style-type";
        public const string LIST_STYLE_IMAGE = "list-style-image"; 
        public const string LIST_STYLE_POSITION = "list-style-position"; 
        public const string LIST_STYLE = "list-style";
    
        // CSS 2.1 - Colors and Backgrounds
        public const string FOREGROUND_COLOR = "color";
        public const string BACKGROUND_COLOR = "background-color";
        public const string BACKGROUND_IMAGE = "background-image";
        public const string BACKGROUND_REPEAT = "background-repeat";
        public const string BACKGROUND_ATTACHMENT = "background-attachment";
        public const string BACKGROUND_POSITION = "background-position";
        public const string SHORT_BACKGROUND = "background";
    
        // CSS 2.1 - Fonts
        public const string FONT_FAMILY = "font-family";
        public const string FONT_STYLE = "font-style";
        public const string FONT_VARIANT = "font-variant";
        public const string FONT_WEIGHT = "font-weight";
        public const string FONT_SIZE = "font-size";
        public const string SHORT_FONT = "font";
    
        // CSS 2.1 - Text
        public const string TEXT_INDENT = "text-indent";
        public const string TEXT_ALIGN = "text-align";
        public const string TEXT_DECORATION = "text-decoration";
        public const string LETTER_SPACING = "letter-spacing";
        public const string WORD_SPACING = "word-spacing";
        public const string TEXT_TRANSFORM = "text-transform";
        public const string WHITE_SPACE = "white-space";
  
        // CSS 2.1 - Tables
        public const string CAPTION_SIDE = "caption-side";
        public const string TABLE_LAYOUT = "table-layout";
        public const string BORDER_COLLAPSE = "border-collapse"; 
        public const string BORDER_SPACING = "border-spacing";
        public const string EMPTY_CELLS = "empty-cells";
    
        // CSS 2.1 - User interface
        public const string CURSOR = "cursor";
        public const string OUTLINE_WIDTH = "outline-width";
        public const string OUTLINE_STYLE = "outline-style";
        public const string OUTLINE_COLOR = "outline-color";
        public const string SHORT_OUTLINE = "outline";
    }
}
