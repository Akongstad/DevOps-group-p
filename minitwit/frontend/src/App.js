import React from 'react';
import { Routes, Route } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
//import Timeline from './components/Timeline';

export default function App() {

    return (
      <Routes>
          <Route path='/login' element={<Login/>} />
          <Route path='/register' element={<Register/>} />
      </Routes>
    );
}
