﻿
namespace ConscriptDesigner.Anchorables {
	/// <summary>An interface for an anchorable or document
	/// that requests close without closing.</summary>
	public interface IRequestCloseAnchorable {

		/// <summary>Force-closes the anchorable.</summary>
		void ForceClose();

		/// <summary>Closes the anchorable with a request.</summary>
		void Close();

		/// <summary>Gets or sets if the anchorable is active.</summary>
		bool IsActive { get; set; }

		/// <summary>Focuses on the anchorable's content.</summary>
		void Focus();
	}
}
