package md5262e42aa820b59232b51d7a755538bd4;


public class _activity_dayview_helper
	extends android.widget.BaseAdapter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getCount:()I:GetGetCountHandler\n" +
			"n_getItem:(I)Ljava/lang/Object;:GetGetItem_IHandler\n" +
			"n_getItemId:(I)J:GetGetItemId_IHandler\n" +
			"n_getView:(ILandroid/view/View;Landroid/view/ViewGroup;)Landroid/view/View;:GetGetView_ILandroid_view_View_Landroid_view_ViewGroup_Handler\n" +
			"";
		mono.android.Runtime.register ("MyAggieNew._activity_dayview_helper, MyAggieNew", _activity_dayview_helper.class, __md_methods);
	}


	public _activity_dayview_helper ()
	{
		super ();
		if (getClass () == _activity_dayview_helper.class)
			mono.android.TypeManager.Activate ("MyAggieNew._activity_dayview_helper, MyAggieNew", "", this, new java.lang.Object[] {  });
	}

	public _activity_dayview_helper (android.content.Context p0, java.lang.String[] p1, java.lang.String[] p2, android.graphics.Bitmap[] p3, java.lang.String[] p4, java.lang.String[] p5, android.graphics.Bitmap[] p6, java.lang.String[] p7, java.lang.String[] p8)
	{
		super ();
		if (getClass () == _activity_dayview_helper.class)
			mono.android.TypeManager.Activate ("MyAggieNew._activity_dayview_helper, MyAggieNew", "Android.Content.Context, Mono.Android:System.String[], mscorlib:System.String[], mscorlib:Android.Graphics.Bitmap[], Mono.Android:System.String[], mscorlib:System.String[], mscorlib:Android.Graphics.Bitmap[], Mono.Android:System.String[], mscorlib:System.String[], mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8 });
	}


	public int getCount ()
	{
		return n_getCount ();
	}

	private native int n_getCount ();


	public java.lang.Object getItem (int p0)
	{
		return n_getItem (p0);
	}

	private native java.lang.Object n_getItem (int p0);


	public long getItemId (int p0)
	{
		return n_getItemId (p0);
	}

	private native long n_getItemId (int p0);


	public android.view.View getView (int p0, android.view.View p1, android.view.ViewGroup p2)
	{
		return n_getView (p0, p1, p2);
	}

	private native android.view.View n_getView (int p0, android.view.View p1, android.view.ViewGroup p2);

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
