# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrang.configure("2") do |config|

  config.vm.box = 'digital_ocean'
  config.vm.box_url = 'https://github.com/devopsgroup-io/vagrant-digitalocean/raw/master/box/digital_ocean.box'
  config.ssh.private_key_path = '~/.ssh/id_ed25519'
  config.vm.synced_folder "./minitwit/compose.dev.yaml", "/app/compose.dev.yaml", type: "rsync"

  config.vm.define "webserver", primary: true do |server|

    server.vm.provider :digital_ocean do |provider|
      provider.ssh_key_name = ENV["SSH_KEY_NAME"]
      provider.token = ENV["DIGITAL_OCEAN_TOKEN"]
      provider.image = 'ubuntu-18-04-x64'
      provider.region = 'fra1'
      provider.size = 's-1vcpu-1gb'
      provider.privatenetworking = true
    end

    server.vm.provision "shell", inline: <<-SHELL
      sudo apt install docker.io
      sudo systemctl start docker
      sudo systemctl enable docker
      sudo curl -L https://github.com/docker/compose/releases/download/2.2.3/docker-compose-`uname -s`-`uname -m` -o /usr/local/bin/docker-compose
      sudo chmod +x /usr/local/bin/docker-compose
      
      docker pull virginity/minitwit_backend:latest
      docker pull virginity/minitwit_frontend:latest

      docker-compose -f /app/compose.dev.yaml up
      
