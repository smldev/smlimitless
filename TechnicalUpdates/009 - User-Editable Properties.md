# Super Mario Limitless

## Technical Update #009 - Properties

This document will cover:
* The old method of dealing with user-editable properties, and its shortcomings.
* A set of new attributes that decorate types with user-editable properties and the properties themselves.
* A method by which `System.Windows.Forms` controls can dynamically generate controls for user-editable properties and correctly bind them.
* How the editor form will handle these dynamically generated controls.

### Description

#### The `PropertyGrid` and its Shortcomings

The `PropertyGrid` control is a Windows Forms control used extensively in Visual Studio to edit properties on controls and project files. It is a two-column grid that shows property names on the left and their values on the right. Users can modify the properties' values by editing the the values on the right column. This control can be used in any Windows Form and can be given any object to display its properties.

This is a very powerful control with some pretty heavy drawbacks; any exotic properties (such as Vector2s and Rectangles) need to have a type converter to be able to display it as a richly editable object, such as one with nested properties. Threads abound on the Internet with people asking questions on how to use the PropertyGrid for their objects.

One of the key goals of Limitless is that its users, especially custom assembly developers, shouldn't have to write large amounts of boilerplate code just to be able to edit properties in the level editor. As a result, we're going in a different direction with properties.

#### Attributes for User-Editable Properties

The following attributes will be added to the `SMLimitless.Editor.Attributes` namespace:

* `HasEditablePropertiesAttribute`
* `BooleanPropertyAttribute` with arguments `string name`, `string description`
* `IntegerPropertyAttribute` with arguments `string name`, `string description`, `int minValue`, `int maxValue`
* `LongIntegerPropertyAttribute` with arguments `string name`, `string description`, `long minValue`, `long maxValue`
* `FloatingPointPropertyAttribute` with arguments `string name`, `string description`, `float minValue', `float maxValue`
* `DoublePropertyAttribute` with arguments `string name`, `string description`, `double minValue`, `double maxValue`
* `Vector2PropertyAttribute` with arguments `string name`, `string description`
* `PointPropertyAttribute` with arguments `string name`, `string description`
* `BoundingRectanglePropertyAttribute` with arguments `string name`, `string description`
* `RectanglePropertyAttribute` with arguments `string name`, `string description`
* `ColorPropertyAttribute` with arguments `string name`, `string description`
* `StringPropertyAttribute` with arguments `string name`, `string description`
* `NestedPropertyAttribute` with arguments `string name`, `string description`

Why are there so many different attributes? While something like a `PropertyAttribute(Type, ...)` would work, not every type can easily have controls generated to edit it. The above types represent most of the property types that developers would need most of the time, and, in a pinch, the `StringPropertyAttribute` can be used to send stringly-typed data.

The `HasEditablePropertiesAttribute` attribute is to only be applied to types. Only types decorated with this attribute will be searched for user-editable properties.

The `NestedPropertyAttribute` is applied to any property that is decorated with the `HasEditablePropertiesAttribute`. Limitless will search for properties in this property decorated with any of the above attributes and dynamically generate controls for them (see below). Controls for nested properties are only generated when the user opens a property form, so infinite loops are not a concern.

All the other `...PropertyAttribute` attributes are applied to properties of those types. These properties must be public and at least have a getter. If any of these attributes decorate properties that aren't the correct type, an exception is thrown at runtime.

Read-only properties will display the information of the property but all fields will be disabled and cannot be changed. Additionally, controls for properties will not update with new values, as the level editor should be the only one to change such properties while the game is in editor mode.

These attributes define a few arguments: name, description, `minValue` and `maxValue`. The name is displayed as the label on the property form, the description resides in the tooltip shown when mousing over the label, and the `minValue` and `maxValue` define a reasonable range for the property.
 
#### Dynamic Control Generation

There are three locations in the editor forms that display properties: the level's properties, the section's properties, and the currently selected sprite or tile's properties. When any of these objects need their properties displayed, Limitless will check the object's type for the `HasEditablePropertiesAttribute`. Types without this attribute will not display any properties.

The `DynamicPropertyControlGenerator` static class in the `SMLimitless.Editor` namespace will generate controls on a `Panel` instance provided to the `void GenerateControls(Panel, object)` method. This method scans all the properties as described below. Nested properties are scanned but not recursively; only the top level properties are scanned.

For types with this attribute, Limitless will check every property (including non-public and static properties), in alphabetical descending order, for any of the `...PropertyAttribute` attributes, and generate controls on a scrollable `Panel`. Any property not decorated with an attribute will be ignored. Any property with an attribute that is private, protected, internal, or protected internal will cause Limitless to throw an exception stating that user-editable properties must be public. Read-only properties will have read-only (disabled) controls generated for them. Write-only properties cause Limitless to throw an exception stating that user-editable controls cannot be write-only.

Limitless will generate controls for each property along a single row. New controls will be placed on a new row, with the rows extending continuously downward on a `Panel` that has a scrollbar, much like the `PhysicsSettingEditorForm`.

Each user-editable property will have a `Label` instance that displays the name of the property. The label will display a tooltip on mouseover which contains the description of the property and its type. Each property will also have an `Action<string, ...>` (arguments: property name, new value) called from the applicable event handler that, after user input validation, sets the property. Each property will have a "Set" button that commits changes to the property. Each property will have controls generated for it as follows:

* `BooleanPropertyAttribute`: Two radio buttons "True" and "False".
* `IntegerPropertyAttribute`: A `NumericUpDown` control.
* `LongIntegerPropertyAttribute`: A `TextBox` into which a number can be typed.
* `FloatingPointPropertyAttribute`: A `TextBox` into which a number can be typed.
* `DoublePropertyAttribute`: A `TextBox` into which a number can be typed.
* `Vector2PropertyAttribute`: Two labels "X" and "Y", and two `TextBox` controls into which numbers can be typed.
* `PointPropertyAttribute`: Two labels "X" and "Y", and two `NumericUpDown` controls.
* `BoundingRectanglePropertyAttribute`: Four labels "X", "Y", "Width", and "Height" and four `TextBox` controls into which numbers can be typed.
* `RectanglePropertyAttribute`: Four labels "X", "Y", "Width", and "Height" and four `NumericUpDown` controls.
* `ColorPropertyAttribute`: Four labels "R", "G", "B", and "A", four `NumericUpDown` controls, and a small panel that displays the currently selected color.
* `StringPropertyAttribute`: A `TextBox`.
* `NestedPropertyAttribute`: A button with an ellipsis. Clicking this button opens another `PropertyForm`, populated by a call to `DynamicPropertyControlGenerator.GenerateControls(Panel, object)` that displays the properties of the nested property.

#### Changes to `EditorForm` and `PropertyForm`

The `PropertyGrid` instances already present in these two forms will be replaced with `Panel` instances. When an object is selected with the `EditorSelectedObject`, the forms will call the `DynamicPropertyControlGenerator.GenerateControls(Panel, object)` method to populate the panel.

### Implementation

* New namespace `SMLimitless.Editor.Attributes`
  * Sealed class `[AttributeUsage(inherited = true)] HasEditablePropertiesAttribute : Attribute`
  * Sealed classes, all with attribute `[AttributeUsage(inherited = true)]` and deriving from `Attribute`:
    * `BooleanPropertyAttribute`
    * `IntegerPropertyAttribute`
    * `LongIntegerPropertyAttribute`
    * `FloatingPointPropertyAttribute`
    * `DoublePropertyAttribute`
    * `Vector2PropertyAttribute`
    * `PointPropertyAttribute`
    * `BoundingRectanglePropertyAttribute`
    * `RectanglePropertyAttribute`
    * `ColorPropertyAttribute`
    * `StringPropertyAttribute`
    * `NestedPropertyAttribute`
    * All types have constructor arguments `string name, string description`
    * `IntegerPropertyAttribute`, `LongIntegerPropertyAttribute`, `FloatingPointPropertyAttribute`, and `DoublePropertyAttribute` have constructor arguments `T minValue, T maxValue`
* In namespace `SMLimitless.Editor`
  * Static class `DynamicPropertyControlGenerator`
    * With static method `void GenerateControls(Panel, object)`