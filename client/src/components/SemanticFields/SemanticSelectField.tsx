import React from "react";
import { DropdownItemProps, Form as SemanticForm } from "semantic-ui-react";
import { useField } from "formik";

interface SemanticSelectFieldProps {
  label: string;
  name: string;
  [key: string]: any;
  options: DropdownItemProps[];
}

export const SemanticSelectField = ({
  label,
  ...props
}: SemanticSelectFieldProps) => {
  const [field, meta] = useField(props);

  return (
    <SemanticForm.Select
      {...field}
      {...props}
      label={label}
      error={meta.touched && meta.error ? meta.error : null}
    />
  );
};
