# How to display custom field values in resource headers


<p>This example illustrates how to display custom field values in resource headers. <br />
A basic approach for customizing a resource header style is shown in Vertical/Horizontal Resource Styles modules of the SchedulerMainDemo, shipped along with DXScheduler for WPF suite. This approach is based on changing the corresponding template as style characteristics dictate.<br />
To modify the template so it displays custom field values, you should implement an appropriate value converter type and bind the UI item (TextBlock in this example) to the <strong>ResourceId </strong>property in the <strong>VerticalResourceHeaderStyle</strong> template, using your converter. Pay attention to the technique used in the <strong>MainWindow.ApplyResourceHeadersStyle</strong> method to apply a custom style.</p><p><strong>See also:</strong><br />
<a href="http://documentation.devexpress.com/#WPF/DevExpressXpfSchedulerSchedulerViewBase_VerticalResourceHeaderStyletopic"><u>SchedulerViewBase.VerticalResourceHeaderStyle Property</u></a></p>

<br/>


