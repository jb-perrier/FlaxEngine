// Copyright (c) 2012-2021 Wojciech Figat. All rights reserved.

using System;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI
{
    /// <summary>
    /// Tool strip button control.
    /// </summary>
    /// <seealso cref="FlaxEngine.GUI.Control" />
    [HideInEditor]
    public class ToolStripButton : Control
    {
        /// <summary>
        /// The default margin for button parts (icon, text, etc.).
        /// </summary>
        public const int DefaultMargin = 2;

        private SpriteHandle _icon;
        private string _text;
        private bool _mouseDown;

        /// <summary>
        /// Event fired when user clicks the button.
        /// </summary>
        public Action Clicked;

        /// <summary>
        /// The checked state.
        /// </summary>
        public bool Checked;

        /// <summary>
        /// The automatic check mode.
        /// </summary>
        public bool AutoCheck;

        /// <summary>
        /// Gets or sets the button text.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                PerformLayout();
            }
        }

        /// <summary>
        /// The icon.
        /// </summary>
        public SpriteHandle Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                PerformLayout();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripButton"/> class.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="icon">The icon.</param>
        public ToolStripButton(float height, ref SpriteHandle icon)
        : base(0, 0, height, height)
        {
            _icon = icon;
        }

        /// <summary>
        /// Sets the automatic check mode.
        /// </summary>
        /// <param name="value">True if use auto check, otherwise false.</param>
        /// <returns>This button.</returns>
        public ToolStripButton SetAutoCheck(bool value)
        {
            AutoCheck = value;
            return this;
        }

        /// <summary>
        /// Sets the checked state.
        /// </summary>
        /// <param name="value">True if check it, otherwise false.</param>
        /// <returns>This button.</returns>
        public ToolStripButton SetChecked(bool value)
        {
            Checked = value;
            return this;
        }

        /// <inheritdoc />
        public override void Draw()
        {
            base.Draw();

            // Cache data
            var style = Style.Current;
            float iconSize = Height - DefaultMargin;
            var clientRect = new Rectangle(Vector2.Zero, Size);
            var iconRect = new Rectangle(DefaultMargin, DefaultMargin, iconSize, iconSize);
            var textRect = new Rectangle(DefaultMargin, 0, 0, Height);
            bool enabled = EnabledInHierarchy;

            // Draw background
            if (enabled && (IsMouseOver || Checked))
                Render2D.FillRectangle(clientRect, Checked ? style.BackgroundSelected : _mouseDown ? style.BackgroundHighlighted : (style.LightBackground * 1.3f));

            // Draw icon
            if (_icon.IsValid)
            {
                Render2D.DrawSprite(_icon, iconRect, enabled ? style.Foreground : style.ForegroundDisabled);
                textRect.Location.X += iconSize + DefaultMargin;
            }

            // Draw text
            if (!string.IsNullOrEmpty(_text))
            {
                textRect.Size.X = Width - DefaultMargin - textRect.Left;
                Render2D.DrawText(
                    style.FontMedium,
                    _text,
                    textRect,
                    enabled ? style.Foreground : style.ForegroundDisabled,
                    TextAlignment.Near,
                    TextAlignment.Center);
            }
        }

        /// <inheritdoc />
        public override void PerformLayout(bool force = false)
        {
            var style = Style.Current;
            float iconSize = Height - DefaultMargin;
            bool hasSprite = _icon.IsValid;
            float width = DefaultMargin * 2;

            if (hasSprite)
                width += iconSize;
            if (!string.IsNullOrEmpty(_text) && style.FontMedium)
                width += style.FontMedium.MeasureText(_text).X + (hasSprite ? DefaultMargin : 0);

            Width = width;
        }

        /// <inheritdoc />
        public override bool OnMouseDown(Vector2 location, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                // Set flag
                _mouseDown = true;

                Focus();
                return true;
            }

            return base.OnMouseDown(location, button);
        }

        /// <inheritdoc />
        public override bool OnMouseUp(Vector2 location, MouseButton button)
        {
            if (button == MouseButton.Left && _mouseDown)
            {
                // Clear flag
                _mouseDown = false;

                // Fire events
                if (AutoCheck)
                    Checked = !Checked;
                Clicked?.Invoke();
                (Parent as ToolStrip)?.OnButtonClicked(this);

                return true;
            }

            return base.OnMouseUp(location, button);
        }

        /// <inheritdoc />
        public override void OnMouseLeave()
        {
            // Clear flag
            _mouseDown = false;

            base.OnMouseLeave();
        }

        /// <inheritdoc />
        public override void OnLostFocus()
        {
            // Clear flag
            _mouseDown = false;

            base.OnLostFocus();
        }
    }
}
