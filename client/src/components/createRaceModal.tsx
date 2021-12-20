import React, { useEffect, useReducer } from "react";
import { Button, Form, Modal } from "semantic-ui-react";
import { Formik } from "formik";
import * as Yup from "yup";
import { SemanticTextInputField } from "./SemanticFields/SemanticTextInputField";
import { SemanticDatePickerInputField } from "./SemanticFields/SemanticDatePickerInputField";
import { IRace } from "../common";

interface CreateRaceModalProps {
  // If this modal is currently being displayed
  open: boolean;
  // Method to call when the modal should be closed
  handleClose: () => void;

  // Method to call when the modal is submitting its form
  onSubmit: (race: Partial<IRace>) => void;

  // The initial IRace to populate the form with
  initialRace: Partial<IRace>;

  // If this form is for creating a new distance for an existing race event
  distanceOnly: boolean;

  // The text to display in the header of the modal
  header: string;
}

const CreateRaceModal = (props: CreateRaceModalProps) => {
  const [, forceUpdate] = useReducer((x) => x + 1, 0);

  return (
    <Formik
      initialValues={{
        name: props.initialRace.name,
        date: props.initialRace.date,
        location: props.initialRace.location,
        distance: props.initialRace.distance,
      }}
      validationSchema={Yup.object({
        name: Yup.string()
          .not(["Add New"], "Entered name is invalid.")
          .required("This field is required."),
        date: Yup.date()
          .nullable()
          .required("This field is required."),
        location: Yup.string().required("This field is required."),
        distance: Yup.string().required("This field is required."),
      })}
      onSubmit={(values) => {
        var createdRace: Partial<IRace> = {
          name: values.name as string,
          distance: values.distance as string,
          date: values.date as Date,
          location: values.location as string,
        };

        props.onSubmit(createdRace);
      }}
    >
      {({ handleSubmit, resetForm }) => {
        //
        // When this modal is open, we always want a blank form.
        // We can use an effect to accomplish this.
        //

        useEffect(() => {
          if (props.open) {
            resetForm();
          }
        }, [props.open]);

        return (
          <Modal
            centered={false}
            open={props.open}
            onClose={props.handleClose}
            dimmer="blurring"
            size="fullscreen"
          >
            <Modal.Header>{props.header}</Modal.Header>
            <Modal.Content>
              <Modal.Description>
                <Form onSubmit={handleSubmit}>
                  <Form.Group widths="equal">
                    <SemanticTextInputField
                      label="Race Name"
                      name="name"
                      disabled={props.distanceOnly}
                      placeholder="My Race"
                    />
                  </Form.Group>
                  <Form.Group widths="equal">
                    <SemanticTextInputField
                      label="Race Location"
                      name="location"
                      disabled={props.distanceOnly}
                      placeholder="Redmond, WA"
                    />
                    <SemanticDatePickerInputField
                      label="Race Date"
                      name="date"
                      disabled={props.distanceOnly}
                      clearable={!props.distanceOnly}
                      placeholder="YYYY-MM-DD"
                    />
                  </Form.Group>
                  <Form.Group widths="equal">
                    <SemanticTextInputField
                      label="Race Distance"
                      name="distance"
                      placeholder="10K"
                    />
                  </Form.Group>
                </Form>
              </Modal.Description>
            </Modal.Content>
            <Modal.Actions>
              <Button secondary onClick={() => props.handleClose()}>
                Cancel
              </Button>
              <Button primary type="button" onClick={() => handleSubmit()}>
                Save
              </Button>
            </Modal.Actions>
          </Modal>
        );
      }}
    </Formik>
  );
};

export default CreateRaceModal;
