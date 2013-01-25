using System.Windows.Forms;

namespace Szotar.WindowsForms {
	/// <summary>
	/// Exposes methods to ensure only a single instance of a form exists.
	/// </summary>
	public static class ShowForm {
		/// <summary>
		/// Brings an existing instance of the form to the foreground,
		/// or if there is no existing instance, creates a new one using the default
		/// constructor.
		/// </summary>
		/// <typeparam name="T">The type of form to show.</typeparam>
		/// <returns>The new or existing form.</returns>
		public static T Show<T>()
			where T : Form, new()
		{
			return Show(any => true, () => new T());
		}

		/// <summary>
		/// Brings an existing instance of the form to the foreground,
		/// or if there is no existing instance, creates a new one.
		/// </summary>
		/// <typeparam name="T">The type of form to show.</typeparam>
		/// <param name="create">A function that creates a new form.</param>
		/// <returns>The new or existing form.</returns>
		public static T Show<T>(Func<T> create)
			where T : Form
		{
			return Show(any => true, create);
		}

		/// <summary>
		/// Brings a suitable existing instance of the form to 
		/// the foreground, or if there is no suitable existing instance, 
		/// creates a new one.
		/// </summary>
		/// <typeparam name="T">The type of form to show.</typeparam>
		/// <param name="predicate">A function that decides if an existing 
		/// form is suitable to be shown.</param>
		/// <param name="create">A function that creates a new form.</param>
		/// <returns>The new or existing form.</returns>
		public static T Show<T>(Func<bool, T> predicate, Func<T> create)
			where T : Form
		{
			foreach (Form f in Application.OpenForms) {
				var form = f as T;
				if (form != null && predicate(form)) {
					form.Show();
					form.BringToFront();
					return form;
				}
			}

			var newForm = create();

            // Could have closed itself due to some error.
            if (!newForm.IsDisposed)
			    newForm.Show();
			return newForm;
		}
	}
}