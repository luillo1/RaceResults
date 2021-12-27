import React from "react";
import { Button, Form as SemanticForm } from "semantic-ui-react";
import { useField, useFormikContext } from "formik";
import DatePicker, { ReactDatePickerCustomHeaderProps } from "react-datepicker";

import "react-datepicker/dist/react-datepicker.css";

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
    <SemanticForm.Field
      {...props}
      label={label}
      name={props.name}
      className="date-picker-field"
      control={DatePicker}
      clearOnSameDateClick={true} // this is needed to ensure the text is reset when resetForm is called (due a bug in the component's source)
      onChange={(date: Date) => {
        context.setFieldValue(field.name, date);
      }}
      onBlur={() => {
        context.setFieldTouched(field.name, true);
      }}
      todayButton={
        <div style={{ marginLeft: "5px", marginRight: "5px" }}>
          <Button
            primary
            fluid
            onClick={(event: React.MouseEvent<HTMLButtonElement>) => {
              event.preventDefault();
            }}
            content="Today"
            size={"tiny"}
          />
        </div>
      }
      selected={field.value}
      error={meta.touched && meta.error ? meta.error : null}
      renderCustomHeader={({
        date,
        decreaseMonth,
        increaseMonth,
        prevMonthButtonDisabled,
        nextMonthButtonDisabled,
      }: ReactDatePickerCustomHeaderProps) => (
        <div
          style={{
            margin: 10,
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            fontSize: "1rem",
          }}
        >
          <Button
            basic
            onClick={(event: React.MouseEvent<HTMLButtonElement>) => {
              event.preventDefault();
              decreaseMonth();
            }}
            disabled={prevMonthButtonDisabled}
            icon="angle left"
            size={"tiny"}
          />

          <span>
            {date.toLocaleString("default", { month: "long", year: "numeric" })}
          </span>

          <Button
            basic
            onClick={(event: React.MouseEvent<HTMLButtonElement>) => {
              event.preventDefault();
              increaseMonth();
            }}
            disabled={nextMonthButtonDisabled}
            icon="angle right"
            size={"tiny"}
          />
        </div>
      )}
    />
  );
};
