﻿using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace CommentRecall
{
    [Export(typeof(AdornmentLayerDefinition))]
    [Name("CommentAdornmentLayer")]
    [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
    public AdornmentLayerDefinition commentLayerDefinition;

    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public sealed class Connector : IWpfTextViewCreationListener
    {
        public void TextViewCreated(IWpfTextView textView)
        {
            CommentAdornmentManager.Create(textView);
        }

        static public void Execute(IWpfTextViewHost host)
        {
            IWpfTextView view = host.TextView;

            // Add a comment on the selected text
            if (!view.Selection.IsEmpty)
            {
                // Get the provider for the comment adornments in the property bag of the view
                CommentAdornmentProvider provider = view.Properties.GetProperty<CommentAdornmentProvider>(typeof(CommentAdornmentProvider));

                // Add some arbitrary author and comment text
                string author = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string comment = "Four score...";

                // Add the comment adonrment using the provider
                provider.Add(view.Selection.SelectedSpans[0], author, comment);
            }
        }
    }
}
