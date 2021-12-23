import React from "react";
import { Form as SemanticForm } from "semantic-ui-react";
import { useField } from "formik";

/* eslint-disable  @typescript-eslint/no-explicit-any */
interface SemanticTextInputFieldProps {
  label: string;
  name: string;
  [key: string]: any;
}

export const SemanticTextInputField = ({
  label,
  ...props
}: SemanticTextInputFieldProps) => {
  const [field, meta] = useField(props);

  return (
    <SemanticForm.Field
      {...field}
      {...props}
      label={label}
      name={props.name}
      control="input"
      error={meta.touched && meta.error ? meta.error : null}
    />
  );
};
