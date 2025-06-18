
import React, { useState } from 'react';
import { CreateFlight } from '../types/Flight';
import './FlightForm.css';

interface FlightFormProps {
  onSubmit: (flight: CreateFlight) => void;
}

export const FlightForm: React.FC<FlightFormProps> = ({ onSubmit }) => {
  const [formData, setFormData] = useState<CreateFlight>({
    flightNumber: '',
    destination: '',
    departureTime: '',
    gate: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.flightNumber.trim()) {
      newErrors.flightNumber = 'Flight number is required';
    }
    if (!formData.destination.trim()) {
      newErrors.destination = 'Destination is required';
    }
    if (!formData.gate.trim()) {
      newErrors.gate = 'Gate is required';
    }
    if (!formData.departureTime) {
      newErrors.departureTime = 'Departure time is required';
    } else {
      const departureDate = new Date(formData.departureTime);
      if (departureDate <= new Date()) {
        newErrors.departureTime = 'Departure time must be in the future';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validate()) {
      onSubmit(formData);
      // Reset form
      setFormData({
        flightNumber: '',
        destination: '',
        departureTime: '',
        gate: '',
      });
    }
  };

  // Get minimum datetime (current time)
  const minDateTime = new Date().toISOString().slice(0, 16);

  return (
    <form onSubmit={handleSubmit} className="flight-form">
      <h2>Add New Flight</h2>
      <div className="form-group">
        <label htmlFor="flightNumber">Flight Number:</label>
        <input
          type="text"
          id="flightNumber"
          name="flightNumber"
          value={formData.flightNumber}
          onChange={handleChange}
          className={errors.flightNumber ? 'error' : ''}
        />
        {errors.flightNumber && <span className="error-message">{errors.flightNumber}</span>}
      </div>

      <div className="form-group">
        <label htmlFor="destination">Destination:</label>
        <input
          type="text"
          id="destination"
          name="destination"
          value={formData.destination}
          onChange={handleChange}
          className={errors.destination ? 'error' : ''}
        />
        {errors.destination && <span className="error-message">{errors.destination}</span>}
      </div>

      <div className="form-group">
        <label htmlFor="departureTime">Departure Time:</label>
        <input
          type="datetime-local"
          id="departureTime"
          name="departureTime"
          value={formData.departureTime}
          onChange={handleChange}
          min={minDateTime}
          className={errors.departureTime ? 'error' : ''}
        />
        {errors.departureTime && <span className="error-message">{errors.departureTime}</span>}
      </div>

      <div className="form-group">
        <label htmlFor="gate">Gate:</label>
        <input
          type="text"
          id="gate"
          name="gate"
          value={formData.gate}
          onChange={handleChange}
          className={errors.gate ? 'error' : ''}
        />
        {errors.gate && <span className="error-message">{errors.gate}</span>}
      </div>

      <button type="submit" className="submit-button">Add Flight</button>
    </form>
  );
};
