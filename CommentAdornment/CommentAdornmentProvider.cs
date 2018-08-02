using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace CommentAdornment
{
    internal class CommentAdornmentProvider
    {
        private ITextBuffer buffer;
        private IList<CommentAdornment> comments = new List<CommentAdornment>();

        public event EventHandler<CommentsChangedEventsArgs> CommentsChanged;

        private CommentAdornmentProvider(ITextBuffer buffer)
        {
            this.buffer = buffer;
            this.buffer.Changed += OnBufferChanged;
        }

        public static CommentAdornmentProvider Create(IWpfTextView view)
        {
            return view.Properties.GetOrCreateSingletonProperty<CommentAdornmentProvider>(delegate { return new CommentAdornmentProvider(view.TextBuffer); });
        }

        public void Detach()
        {
            if (this.buffer != null)
            {
                this.buffer.Changed -= OnBufferChanged;
                this.buffer = null;
            }
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            IList<CommentAdornment> keptComments = new List<CommentAdornment>(this.comments.Count);

            foreach (CommentAdornment comment in this.comments)
            {
                Span span = comment.Span.GetSpan(e.After);

                if (span.Length != 0)
                {
                    keptComments.Add(comment);
                }
            }

            this.comments = keptComments;
        }

        public void Add(SnapshotSpan span, string author, string text)
        {
            if (span.Length == 0)
            {
                throw new ArgumentOutOfRangeException("span");
            }

            if (author == null)
            {
                throw new ArgumentNullException("author");
            }

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            // Create a comment adornment given the span, author and text
            CommentAdornment comment = new CommentAdornment(span, author, text);

            this.comments.Add(comment);

            EventHandler<CommentsChangedEventArgs> commentsChanged = this.CommentsChanged;
            if (CommentsChanged != null)
            {
                commentsChanged(this, new CommentsChangedEventArgs(comment, null));
            }
        }

        public void RemoveComments(SnapshotSpan span)
        {
            EventHandler<CommentsChangedEventArgs> commentsChanged = this.CommentsChanged;

            // Get a list of all the comments that are being kept
            IList<CommentAdornment> keptComments = new List<CommentAdornment>(this.comments.Count);

            foreach(CommentAdornment comment in this.comments)
            {
                // find out if the given span overlaps with the comment text span. If two spans are adjacent, they do not overlap.
                // To consider adjacent spans, use IntersectsWith
                if (comment.Span.GetSpan(span.Snapshot).OverlapsWith(span))
                {
                    // Raise the changed event to delete this comment
                    if (commentsChanged != null)
                    {
                        commentsChanged(this, new CommentsChangedEventArgs(null, comment));
                    }
                }
                else
                {
                    keptComments.Add(comment);
                }

                this.comments = keptComments;
            }
        }
    }
}
