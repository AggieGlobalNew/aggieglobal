package md5262e42aa820b59232b51d7a755538bd4;


public class OnDateSetListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.app.DatePickerDialog.OnDateSetListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDateSet:(Landroid/widget/DatePicker;III)V:GetOnDateSet_Landroid_widget_DatePicker_IIIHandler:Android.App.DatePickerDialog/IOnDateSetListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("MyAggieNew.OnDateSetListener, MyAggieNew", OnDateSetListener.class, __md_methods);
	}


	public OnDateSetListener ()
	{
		super ();
		if (getClass () == OnDateSetListener.class)
			mono.android.TypeManager.Activate ("MyAggieNew.OnDateSetListener, MyAggieNew", "", this, new java.lang.Object[] {  });
	}

	public OnDateSetListener (android.widget.EditText p0)
	{
		super ();
		if (getClass () == OnDateSetListener.class)
			mono.android.TypeManager.Activate ("MyAggieNew.OnDateSetListener, MyAggieNew", "Android.Widget.EditText, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public void onDateSet (android.widget.DatePicker p0, int p1, int p2, int p3)
	{
		n_onDateSet (p0, p1, p2, p3);
	}

	private native void n_onDateSet (android.widget.DatePicker p0, int p1, int p2, int p3);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
