import React, { SyntheticEvent } from "react";
import { Form as SemanticForm } from "semantic-ui-react";
import { useField, useFormikContext } from "formik";
import SemanticDatepicker from "react-semantic-ui-datepickers";
import _ from "lodash";

/* eslint-disable  @typescript-eslint/no-explicit-any */
interface SemanticDatePickerInputFieldProps {
  label: string;
  name: string;
  [key: string]: any;
}

export const SemanticDatePickerInputField = ({
  label,
  name,
  ...props
}: SemanticDatePickerInputFieldProps) => {
  const context = useFormikContext();
  const [field, meta] = useField(name);

  return (
    <SemanticForm.Field
      {..._.omit(field, "onBlur")}
      {...props}
      label={label}
      name={props.name}
      className="date-picker-field"
      control={SemanticDatepicker}
      clearOnSameDateClick={true} // this is needed to ensure the text is reset when resetForm is called (due a bug in the component's source)
      onChange={(_: SyntheticEvent<HTMLElement>, { value }: { value: Date }) => {
        context.setFieldValue(field.name, value);
      }}
      error={meta.touched && meta.error ? meta.error : null}
    />
  );
};
