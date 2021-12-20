import React from "react";
import { Form as SemanticForm } from "semantic-ui-react";
import { useField } from "formik";

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
      control="input"
      error={meta.touched && meta.error ? meta.error : null}
    />
  );
};
