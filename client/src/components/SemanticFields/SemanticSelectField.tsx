import React from "react";
import { DropdownItemProps, Form as SemanticForm } from "semantic-ui-react";
import { useField } from "formik";
import _ from "lodash";

/* eslint-disable  @typescript-eslint/no-explicit-any */
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
      {..._.omit(field, "onBlur")}
      {...props}
      name={props.name}
      label={label}
      error={meta.touched && meta.error ? meta.error : null}
    />
  );
};
