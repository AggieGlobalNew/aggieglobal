package mono.com.github.sundeepk.compactcalendarview;


public class CompactCalendarView_CompactCalendarViewListenerImplementor
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
		mono.android.Runtime.register ("Com.Github.Sundeepk.Compactcalendarview.CompactCalendarView+ICompactCalendarViewListenerImplementor, AndroidBindingCompactCalendarView", CompactCalendarView_CompactCalendarViewListenerImplementor.class, __md_methods);
	}


	public CompactCalendarView_CompactCalendarViewListenerImplementor ()
	{
		super ();
		if (getClass () == CompactCalendarView_CompactCalendarViewListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Github.Sundeepk.Compactcalendarview.CompactCalendarView+ICompactCalendarViewListenerImplementor, AndroidBindingCompactCalendarView", "", this, new java.lang.Object[] {  });
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
