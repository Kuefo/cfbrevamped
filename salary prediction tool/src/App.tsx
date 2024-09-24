import React, { useState } from 'react';
import { Input, InputLabel, MenuItem, Rating, Select } from '@mui/material';

import './App.css';

const formatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
});

const POSITION_RATINGS : { [key: string]: string } = {
  'Head Coach': 'Overall',
  'Offensive Coordinator': 'Offense',
  'Defensive Coordinator': 'Defense'
} as const;

interface Contract {
  position: string,
  prestige: number,
  rating: number,
  offer: number,
  year: number,
}

interface ContractUpdate {
  position?: string,
  prestige?: number,
  rating?: number,
  offer?: number,
  year?: number,
}

export default function App() {
  const defaultValues = {
  	position: 'Head Coach',
  	prestige: 1,
    rating: 65,
    offer: 1,
    year: (new Date()).getFullYear(),
  }
  defaultValues.offer = calculateOffer(defaultValues);

  const [values, setValues] = useState(defaultValues);

  function updateOffer(change: ContractUpdate) {
    const updatedValues = {...values, ...change};

    updatedValues.prestige = Math.min(6, Math.max(1, updatedValues.prestige));
    updatedValues.rating = Math.min(99, Math.max(1, updatedValues.rating));

    updatedValues.offer = calculateOffer(updatedValues);
    setValues(updatedValues);
  }

  function calculateOffer(values: Contract) {
    let predictionRating = 0;
    let predictionPrestige = 0;
    let prediction2013 = 0;

    // Calculate a base value based on the school data.
    if (values.position === "Head Coach") {
      predictionRating = 3508 * Math.exp(0.0754 * values.rating);
      predictionPrestige = (635330 * values.prestige) - 108038;
      prediction2013 = 0.4927441715*predictionRating + 0.5254323619*predictionPrestige - 115931.163
    } else if (values.position === "Offensive Coordinator") {
      predictionRating = 3041 * Math.exp(0.0554 * values.rating);
      predictionPrestige = (84983 * values.prestige) + 56641;
      prediction2013 = 0.4505756884*predictionRating + 0.5633165777*predictionPrestige - 19044.52237;
    } else {
      predictionRating = 1651 * Math.exp(0.0655 * values.rating);
      predictionPrestige = (116478 * values.prestige) + 24404;
      prediction2013 = 0.3586890779*predictionRating + 0.6870167964*predictionPrestige - 29152.95258;
    }

    // Adjust for inflation.
    let predictionInflation = prediction2013 * 2.20589e-36 * Math.exp(0.0485*values.year)/5545890.20;

    // Adjust for the popularity of college football over time.
    let predictionPopularity = predictionInflation * (15650000 / (1 + Math.exp(-0.06 * (values.year - 2023))))/5545479;

    return predictionPopularity;
  }

  return (
    <div className="container">
      <h1>NCAA 14 Contract Estimator</h1>
      <form>
        <InputLabel id="position-label">Position</InputLabel>
        <Select
          labelId="position-label"
          id="position"
          value={values.position}
          label="Position"
          onChange={(e) => {updateOffer({position: e.target.value})}}
          style={{minWidth: "14em"}}
        >
          <MenuItem value={"Head Coach"}>Head Coach</MenuItem>
          <MenuItem value={"Offensive Coordinator"}>Offensive Coordinator</MenuItem>
          <MenuItem value={"Defensive Coordinator"}>Defensive Coordinator</MenuItem>
        </Select>
        <InputLabel id="prestige-label">School Prestige</InputLabel>
        <Rating
          name="prestige"
          max={6}
          value={values.prestige}
          onChange={(e) => {updateOffer({prestige: parseInt((e.target as HTMLInputElement).value)})}}
        />
        <InputLabel id="rating-label">{POSITION_RATINGS[values.position] + " Rating"}</InputLabel>
        <Input
          // @ts-expect-error
          labelid="rating-label"
          id="rating"
          name="rating"
          label="Rating"
          type="number"
          value={values.rating}
          onChange={(e) => {updateOffer({rating: parseInt(e.target.value)})}}
        />
        <InputLabel id="year-label">Year</InputLabel>
        <Input
          // @ts-expect-error
          labelid="year-label"
          id="year"
          name="year"
          label="Year"
          type="number"
          value={values.year}
          onChange={(e) => {updateOffer({year: parseInt(e.target.value)})}}
        />

      </form>
      <h2>Total Offer: {formatter.format(values.offer)}</h2>
    </div>
  );
}
