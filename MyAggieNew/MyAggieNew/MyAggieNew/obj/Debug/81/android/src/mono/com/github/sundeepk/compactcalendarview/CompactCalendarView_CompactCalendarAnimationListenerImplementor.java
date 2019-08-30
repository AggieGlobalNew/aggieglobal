package mono.com.github.sundeepk.compactcalendarview;


public class CompactCalendarView_CompactCalendarAnimationListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.github.sundeepk.compactcalendarview.CompactCalendarView.CompactCalendarAnimationListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onClosed:()V:GetOnClosedHandler:Com.Github.Sundeepk.Compactcalendarview.CompactCalendarView/ICompactCalendarAnimationListenerInvoker, AndroidBindingCompactCalendarView\n" +
			"n_onOpened:()V:GetOnOpenedHandler:Com.Github.Sundeepk.Compactcalendarview.CompactCalendarView/ICompactCalendarAnimationListenerInvoker, AndroidBindingCompactCalendarView\n" +
			"";
		mono.android.Runtime.register ("Com.Github.Sundeepk.Compactcalendarview.CompactCalendarView+ICompactCalendarAnimationListenerImplementor, AndroidBindingCompactCalendarView", CompactCalendarView_CompactCalendarAnimationListenerImplementor.class, __md_methods);
	}


	public CompactCalendarView_CompactCalendarAnimationListenerImplementor ()
	{
		super ();
		if (getClass () == CompactCalendarView_CompactCalendarAnimationListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Github.Sundeepk.Compactcalendarview.CompactCalendarView+ICompactCalendarAnimationListenerImplementor, AndroidBindingCompactCalendarView", "", this, new java.lang.Object[] {  });
	}


	public void onClosed ()
	{
		n_onClosed ();
	}

	private native void n_onClosed ();


	public void onOpened ()
	{
		n_onOpened ();
	}

	private native void n_onOpened ();

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
