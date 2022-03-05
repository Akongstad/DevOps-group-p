import * as React from 'react';
import CssBaseline from '@mui/material/CssBaseline';
import Grid from '@mui/material/Grid';
import Container from '@mui/material/Container';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import Header from './Header';
//import Main from './Main';
import Footer from './Footer';
import Message from "./Message";
import {useEffect, useState} from "react";


const sections = [
    /*{ title: 'Technology', url: '#' },
    { title: 'Design', url: '#' },
    { title: 'Culture', url: '#' },
    { title: 'Business', url: '#' },
    { title: 'Politics', url: '#' },
    { title: 'Opinion', url: '#' },
    { title: 'Science', url: '#' },
    { title: 'Health', url: '#' },
    { title: 'Style', url: '#' },
    { title: 'Travel', url: '#' },*/
];

const demoMessage = {
    username: 'Elon Musk',
    content:
        "Multiple lines of text that form the lede, informing new readers quickly and efficiently about what's most interesting in this post's contents.",
    linkText: 'Continue reading…',
};

const posts1 = [
    {
        username: 'Elon Musk',
        date: 'Nov 12',
        content:
            'This is a wider card with supporting text below as a natural lead-in to additional content.',
        avatar: '',
    },
    {
        username: 'Jeff Bezos',
        date: 'Nov 12',
        content:
            'This is a wider card with supporting text below as a natural lead-in to additional content.',
        avatar: '',
    },
];

const theme = createTheme();

export default function Timeline() {
    let headers = new Headers();
    headers.append('Content-Type', 'application/json');
    //headers.append('Accept', 'application/json');
    headers.append('Origin','http://localhost:3000');
    let url = `http://localhost:5229/minitwitsimulation/msgs`
    const [posts, setPosts] = useState([]);
    const [isLoading, setIsLoading] = useState(true);

    const fetchPost = async () => {
        const response = await fetch(
            url,
        {
            mode: 'cors',
            method: "GET",
            headers: headers
        });
        const data = await response.json();
        setPosts(data);
        setIsLoading(false);
    };
    useEffect(() => {
        fetchPost();
    }, []);
    
    return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <Container maxWidth="lg">
                <Header title="Minitwit" sections={sections}/>
                <Grid container spacing={2}  justifyContent={"center"}>
                    { isLoading ? null :
                        posts.map((post) => (
                        <Message key={post.user} post={post}/>
                        ))
                    }
                    
                </Grid>
            </Container>
            <Footer
                title="TBD Minitwit"
                description="Created for the course: DevOps, Software Evolution and Software Maintenance 2022. IT University of Copenhagen"
            />
        </ThemeProvider>
    );
}