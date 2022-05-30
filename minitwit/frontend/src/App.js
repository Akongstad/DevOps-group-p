import React from 'react';
import { Routes, Route } from 'react-router-dom';
import Register from './components/MUIRegister';
import SignIn from "./components/MUILogin";
import Timeline from './components/Timeline/Timeline';
import UserTimeline from "./components/Timeline/UserTimeline";


export default function App() {
    return (
      <Routes>
          <Route path='/signin' element={<SignIn/>} />
          <Route path='/register' element={<Register/>} />
          <Route path='/' element={<Timeline/>} />
          <Route path="/:Username" element={<UserTimeline/>} />
      </Routes>
    );
}
