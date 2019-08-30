package md5262e42aa820b59232b51d7a755538bd4;


public class ReceivedMessageHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("MyAggieNew.ReceivedMessageHolder, MyAggieNew", ReceivedMessageHolder.class, __md_methods);
	}


	public ReceivedMessageHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == ReceivedMessageHolder.class)
			mono.android.TypeManager.Activate ("MyAggieNew.ReceivedMessageHolder, MyAggieNew", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
	}

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
