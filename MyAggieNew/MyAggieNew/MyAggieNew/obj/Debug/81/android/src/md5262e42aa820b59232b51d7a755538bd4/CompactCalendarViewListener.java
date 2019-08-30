package md5262e42aa820b59232b51d7a755538bd4;


public class CompactCalendarViewListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.github.sundeepk.compactcalendarview.CompactCalendarView.CompactCalendarViewListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDayClick:(Ljava/util/Date;)V:GetOnDayClick_Ljava_util_Date_Handler:Com.Github.Sundeepk.Compactcalendarview.CompactCalendarView/ICompactCalendarViewListenerInvoker, AndroidBindingCompactCalendarView\n" +
			"n_onMonthScroll:(Ljava/util/Date;)V:GetOnMonthScroll_Ljava_util_Date_Handler:Com.Github.Sundeepk.Compactcalendarview.CompactCalendarView/ICompactCalendarViewListenerInvoker, AndroidBindingCompactCalendarView\n" +
			"";
		mono.android.Runtime.register ("MyAggieNew.CompactCalendarViewListener, MyAggieNew", CompactCalendarViewListener.class, __md_methods);
	}


	public CompactCalendarViewListener ()
	{
		super ();
		if (getClass () == CompactCalendarViewListener.class)
			mono.android.TypeManager.Activate ("MyAggieNew.CompactCalendarViewListener, MyAggieNew", "", this, new java.lang.Object[] {  });
	}

	public CompactCalendarViewListener (android.app.Activity p0, android.widget.TextView p1, com.github.sundeepk.compactcalendarview.CompactCalendarView p2)
	{
		super ();
		if (getClass () == CompactCalendarViewListener.class)
			mono.android.TypeManager.Activate ("MyAggieNew.CompactCalendarViewListener, MyAggieNew", "Android.App.Activity, Mono.Android:Android.Widget.TextView, Mono.Android:Com.Github.Sundeepk.Compactcalendarview.CompactCalendarView, AndroidBindingCompactCalendarView", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public void onDayClick (java.util.Date p0)
	{
		n_onDayClick (p0);
	}

	private native void n_onDayClick (java.util.Date p0);


	public void onMonthScroll (java.util.Date p0)
	{
		n_onMonthScroll (p0);
	}

	private native void n_onMonthScroll (java.util.Date p0);

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
