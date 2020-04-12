//
//  MasterViewController.swift
//  helperIOShelpr
//
//  Created by Sam Horn on 4/4/20.
//  Copyright © 2020 Sam Horn. All rights reserved.
//

import UIKit
import CoreData

class MasterViewController: UITableViewController {

    var detailViewController: DetailViewController? = nil
   // var managedObjectContext: NSManagedObjectContext? = (UIApplication.shared.delegate as? AppDelegate)?.persistentContainer.viewContext
    var newThreadView: NewThreadView? = nil
    var threadView: ThreadView? = nil
   // var masterToken: Data? = token
    var threads: [Thread]? = []
    var cellDict: [UITableViewCell: Thread] = [:]
    


    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        //navigationItem.leftBarButtonItem = editButtonItem

        let threadButton = UIBarButtonItem(barButtonSystemItem: .compose, target: self, action: #selector(insertNewObject(_:)))
        //let threadButton = UIBarButtonItem(barButtonSystemItem: .compose, target: self, action: #selector(threadSegue()))
        navigationItem.rightBarButtonItem = threadButton
        if let split = splitViewController {
            let controllers = split.viewControllers
            detailViewController = (controllers[controllers.count-1] as! UINavigationController).topViewController as? DetailViewController
            //detailViewController?.managedObjectContext = managedObjectContext
        }
    }

    override func viewWillAppear(_ animated: Bool) {
        clearsSelectionOnViewWillAppear = splitViewController!.isCollapsed
        super.viewWillAppear(animated)
        
    }
    
    
    override func viewDidAppear(_ animated: Bool) {
        
        super.viewDidAppear(animated)
        let semaphore = DispatchSemaphore (value: 0)
        
        guard let url = URL(string: "https://helpr19api.azurewebsites.net/api/posts/get/threads") else { return }
        
        var request = URLRequest(url: url, timeoutInterval: Double.infinity)
        request.httpMethod = "GET"
        request.addValue("application/json", forHTTPHeaderField: "Content-Type")
        
        let task = URLSession.shared.dataTask(with: request) { data, response, error in
            guard let data = data else {
                print(String(describing: error))
                return
            }
            
            guard (response as! HTTPURLResponse?) != nil else {
                print(String(describing: error))
                return
            }
           // print(String(data: data, encoding: .utf8)!)
            //print("\n\n\nresponse: \(String(describing: response))")
            let decoder = JSONDecoder()
            
            do {
                self.threads = try decoder.decode([Thread].self, from: data)
            } catch {
                print(error.localizedDescription)
            }
            semaphore.signal()
            /*
            var index: Int = 0
            var paths: [IndexPath] = []
            while index < self.threads!.count {
                paths.append(IndexPath(row: index, section: 0))
                index += 1
            }
            self.tableView.insertRows(at: paths, with: .fade)
 */
        }
        
        task.resume()
        semaphore.wait()
       // while
        var index: Int = 0
        var paths: [IndexPath] = []
        while index < self.threads!.count {
            paths.append(IndexPath(row: index, section: 0))
            index += 1
        }
        self.tableView.insertRows(at: paths, with: .fade)
        task.suspend()
      //  tableView.insertRows(at: [)
        //if newThreadView?.wasRecentView ?? true {
          //  let context = self.fetchedResultsController.managedObjectContext
            //let newThread = Thread(context: context)
        
            //newThread.timestamp = Date()
            //newThread.thread_name = newThreadView?.threadText
            
            //let newPost = Post(context: context)
            //newPost.message = newThreadView?.messageText
            //newPost.thread = newThread
            //newThread.addToPosts(newPost)
            /*
            do {
                try context.save()
            } catch {
                // Replace this implementation with code to handle the error appropriately.
                // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
                let nserror = error as NSError
                fatalError("Unresolved error \(nserror), \(nserror.userInfo)")
            }
            newThreadView?.wasRecentView = false
         
 */
//let parameters: [String: String]
        
        
        
    }
     
    
    // func addView() {
    //   insertNewObject(_:)
    // addSubview
    //}
    

    @objc func insertNewObject(_ sender: Any) {
        performSegue(withIdentifier: "newThread", sender: self)
    }
    
    /*
     @objc
     func threadSegue() {
     performSegue(withIdentifier: "newThread", sender: self)
     let context = self.fetchedResultsController.managedObjectContext
     do {
     try context.save()
     } catch {
     // Replace this implementation with code to handle the error appropriately.
     // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
     let nserror = error as NSError
     fatalError("Unresolved error \(nserror), \(nserror.userInfo)")
     }
     }
     */
    /*
     
     @objc
     func insertNewObject(_ sender: Any) {
     let context = self.fetchedResultsController.managedObjectContext
     //let newThread = Thread(context: context)
     
     // If appropriate, configure the new managed object.
     //newThread.timestamp = Date()
     
     // Save the context.
     do {
     try context.save()
     } catch {
     // Replace this implementation with code to handle the error appropriately.
     // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
     let nserror = error as NSError
     fatalError("Unresolved error \(nserror), \(nserror.userInfo)")
     }
     performSegue(withIdentifier: "newThread", sender: self)
     }
     
     */
    
    func update() {
        
    }
    
    func getThread(id: Int) -> Thread? {
        for item in threads! {
            if item.thread_id == id {
                return item
            }
        }
        return nil
    }
    
    // MARK: - Refresh Control
    
   // func configureRefreshControl() {
     //
       // self.refreshControl = UIRefreshControl()
    //}
    
    
    
    // MARK: - Segues
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == "showDetail" {
            if tableView.indexPathForSelectedRow != nil {
                //let object = fetchedResultsController.object(at: indexPath)
                let controller = (segue.destination as! UINavigationController).topViewController as! DetailViewController
               // controller.detailItem = object
               // controller.managedObjectContext = managedObjectContext
                controller.navigationItem.leftBarButtonItem = splitViewController?.displayModeButtonItem
                controller.navigationItem.leftItemsSupplementBackButton = true
                detailViewController = controller
                guard let full_send = sender as! UITableViewCell? else {
                    return
                }
                detailViewController?.thread = threads![tableView.indexPath(for: full_send)!.row]
             //   detailViewController?.token = token
            }
        }
            
        else if segue.identifier == "newThread" {
            let controller = segue.destination as! NewThreadView
            newThreadView = controller
            //threadView = newThreadView?.threadView
            
        }
    }
    
    
    // MARK: - Table View
    
    override func numberOfSections(in tableView: UITableView) -> Int {
       // return fetchedResultsController.sections?.count ?? 0
        return 1
    }

    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        //let sectionInfo = fetchedResultsController.sections![section]
        return threads!.count
    }

    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        let cell = tableView.dequeueReusableCell(withIdentifier: "Thread Cell", for: indexPath) as! ThreadCell
        let thread = (threads?[indexPath.row])!
        //guard let cell = cell as! ThreadCell else {return}
        cell.threadName.text = thread.name
        cellDict[cell] = thread
        return cell
    }

    override func tableView(_ tableView: UITableView, canEditRowAt indexPath: IndexPath) -> Bool {
        // Return false if you do not want the specified item to be editable.
        return true
    }

    override func tableView(_ tableView: UITableView, commit editingStyle: UITableViewCell.EditingStyle, forRowAt indexPath: IndexPath) {
        /*
        if editingStyle == .delete {
            let context = fetchedResultsController.managedObjectContext
            context.delete(fetchedResultsController.object(at: indexPath))
                
            do {
                try context.save()
            } catch {
                // Replace this implementation with code to handle the error appropriately.
                // fatalError() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
                let nserror = error as NSError
                fatalError("Unresolved error \(nserror), \(nserror.userInfo)")
            }
        }
        */
        if editingStyle == .insert {
            
        }
    }
    //override func tableView(_ tableView: UITableView, willDisplay cell: UITableViewCell, forRowAt  //indexPath: IndexPath) {
        
        
    //}
    
    func configureCell(_ cell: ThreadCell, withThread thread: Thread) {
        cell.threadName.text = thread.name
        cellDict[cell] = thread
        
    }
        
     
}


