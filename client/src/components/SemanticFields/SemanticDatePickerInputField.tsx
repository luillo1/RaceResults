import React from "react";
import { Form } from "semantic-ui-react";
import { useField, useFormikContext } from "formik";

import "react-datepicker/dist/react-datepicker.css";
import DatePickerInput from "../DatePickerInput";

/* eslint-disable  @typescript-eslint/no-explicit-any */
interface SemanticDatePickerInputFieldProps {
  label: string;
  name: string;
  [key: string]: any;
}

export const SemanticDatePickerInputField = ({
  label,
  ...props
}: SemanticDatePickerInputFieldProps) => {
  const context = useFormikContext();
  const [field, meta] = useField(props);

  return (
    <Form.Field
      {...props}
      name={props.name}
      label={label}
      control={DatePickerInput}
      onChange={(date: Date) => {
        context.setFieldValue(field.name, date);
      }}
      onBlur={() => {
        context.setFieldTouched(field.name, true);
      }}
      selected={field.value}
      error={meta.touched && meta.error ? meta.error : null}
    />
  );
};
