namespace CommentRecall
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("9679aa82-e955-423d-a761-b80a9d08a025")]
    public class CommentToolWindow : ToolWindowPane, IVsRunningDocTableEvents
    {
        private uint rdtCookie;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentToolWindow"/> class.
        /// </summary>
        public CommentToolWindow() : base(null)
        {
            this.Caption = "CommentToolWindow";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new CommentToolWindowControl();
        }

        protected override void Initialize()
        {
            IVsRunningDocumentTable rdt = (IVsRunningDocumentTable)
            this.GetService(typeof(SVsRunningDocumentTable));
            rdt.AdviseRunningDocTableEvents(this, out rdtCookie);
        }

        protected override void Dispose(bool disposing)
        {
            // Release the RDT cookie.  
            IVsRunningDocumentTable rdt = (IVsRunningDocumentTable)
            Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsRunningDocumentTable));
            rdt.UnadviseRunningDocTableEvents(rdtCookie);

            base.Dispose(disposing);
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            ((CommentToolWindowControl)this.Content).listBox.Items.Add("Entering OnAfterFirstDocumentLock");
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            ((CommentToolWindowControl)this.Content).listBox.Items.Add("Entering OnBeforeLastDocumentUnlock");
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }
    }
}
