> <strong>IMPORTANT</strong>: Starting with version <strong>15.2</strong>, we provide ready-to-use <strong>Editor Attributes</strong>. Please refer to the <a href="https://github.com/DevExpress-Examples/how-to-configure-the-way-properties-are-displayed-and-edited-at-the-data-model-level-t323108">How to: Configure the Way Properties Are Displayed and Edited at the Data Model Level</a> example where we demonstrated this approach. The solution demonstrated in the current example is no longer recommended, as it attempts to emulate the work <strong>PropertyGridControl</strong> performs internally and does not support all scenarios.

<!-- default file list -->
*Files to look at*:

* [Helpers.cs](./CS/Helpers.cs) (VB: [Helpers.vb](./VB/Helpers.vb))
* **[MainWindow.xaml](./CS/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/MainWindow.xaml))**
* [ViewModel.cs](./CS/ViewModel.cs) (VB: [ViewModel.vb](./VB/ViewModel.vb))
<!-- default file list end -->
# PropertyGridControl - How to use various editor types based on custom attribute values


<p>In this example, we dynamically generate the <a href="https://documentation.devexpress.com/#WPF/DevExpressXpfPropertyGridPropertyGridControl_PropertyDefinitionsSourcetopic">PropertyGridControl.PropertyDefinitionsSource</a> and <a href="https://documentation.devexpress.com/#WPF/DevExpressXpfPropertyGridPropertyDefinitionBase_PropertyDefinitionsSourcetopic">PropertyDefinitionBase.PropertyDefinitionsSource</a> collections based on the underlying data source objects. To generate proper definitions, we implemented a custom <strong>PropertyDefinitionTemplateSelector</strong> and select one of the following templates based on the underlying property:<br>1. <strong>CollectionDefinitionTemplate</strong>. We use it if the underlying property is a collection.<br>2. <strong>CustomDefinitionTemplate</strong>. This template is applied only if a specific attribute is set (in this example, we use a custom <strong>CustomEditorAttribute</strong>).<br>3. If the underlying object does not meet the previous conditions, we return <strong>DefinitionTemplate</strong>.<br><br>Finally, there is a custom <a href="https://documentation.devexpress.com/#WPF/DevExpressXpfPropertyGridPropertyDefinition_CellTemplateSelectortopic">CellTemplateSelector</a> in <strong>CustomDefinitionTemplate</strong>. In this selector, we check the <strong>CustomEditorAttribute</strong> attribute value and return one of predefined templates based on it.<br></p>

<br/>


