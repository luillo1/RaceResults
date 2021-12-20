import React from "react";
import { Form as SemanticForm } from "semantic-ui-react";
import { useField } from "formik";

interface SemanticTextAreaFieldProps {
  label: string;
  name: string;
  [key: string]: any;
}

export const SemanticTextAreaField = ({
  label,
  ...props
}: SemanticTextAreaFieldProps) => {
  const [field, meta] = useField(props);

  return (
    <SemanticForm.Field
      {...field}
      {...props}
      label={label}
      control="textarea"
      error={meta.touched && meta.error ? meta.error : null}
    />
  );
};
