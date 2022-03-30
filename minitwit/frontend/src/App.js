import React from 'react';
import { Routes, Route } from 'react-router-dom';
import useToken from './components/App/UseToken';
import Login from './components/Login';
//import Register from './components/Register';
import Register from './components/MUIRegister';
import SignIn from "./components/MUILogin";
import Timeline from './components/Timeline/Timeline';


export default function App() {
    const { token, setToken } = useToken();
    return (
      <Routes>
          <Route path='/signin' element={<SignIn/>} />
          <Route path='/register' element={<Register/>} />
          <Route path='/' element={<Timeline/>} />
      </Routes>
    );
}
